using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.Helpers;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.Search;
using static System.StringComparer;

namespace ToSic.Sxc.Dnn.Search;

/// <summary>
/// This will construct data for the search indexer in DNN.
/// It's created once for each module which will be indexed
/// </summary>
/// <remarks>
/// ATM it's DNN only (because Oqtane doesn't have search indexing)
/// But the code is 99% clean, so it would be easy to split into dnn/Oqtane versions once ready.
/// The only difference seems to be exception logging. 
/// </remarks>
internal class SearchController : ServiceBase
{
    public SearchController(
        AppsCacheSwitch appsCache,
        Generator<CodeCompiler> codeCompiler,
        Generator<CodeRootFactory> codeRootFactory,
        Generator<ISite> siteGenerator,
        LazySvc<IModuleAndBlockBuilder> moduleAndBlockBuilder,
        LazySvc<ILookUpEngineResolver> dnnLookUpEngineResolver,
        EngineFactory engineFactory,
        LazySvc<IAppLoaderTools> loaderTools,
        LazySvc<ILogStore> logStore,
        Polymorphism.Internal.PolymorphConfigReader polymorphism) : base("DNN.Search")
    {
        ConnectServices(
            _appsCache = appsCache,
            _codeCompiler = codeCompiler,
            _codeRootFactory = codeRootFactory,
            _siteGenerator = siteGenerator,
            _engineFactory = engineFactory,
            _loaderTools = loaderTools,
            _dnnLookUpEngineResolver = dnnLookUpEngineResolver,
            _moduleAndBlockBuilder = moduleAndBlockBuilder,
            _logStore = logStore,
            _polymorphism = polymorphism
        );
    }

    private readonly AppsCacheSwitch _appsCache;
    private readonly Generator<CodeCompiler> _codeCompiler;
    private readonly Generator<CodeRootFactory> _codeRootFactory;
    private readonly Generator<ISite> _siteGenerator;
    private readonly EngineFactory _engineFactory;
    private readonly LazySvc<IAppLoaderTools> _loaderTools;
    private readonly LazySvc<ILookUpEngineResolver> _dnnLookUpEngineResolver;
    private readonly LazySvc<IModuleAndBlockBuilder> _moduleAndBlockBuilder;
    private readonly LazySvc<ILogStore> _logStore;
    private readonly Polymorphism.Internal.PolymorphConfigReader _polymorphism;


    /// <summary>
    /// Initialize all values which are needed - or return a text with the info why we must stop. 
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    private string InitAllAndVerifyIfOk(IModule module)
    {
        var l = Log.Fn<string>();
        // Start by getting the module info
        DnnModule = (module as Module<ModuleInfo>)?.GetContents();
        l.A($"start search for mod#{DnnModule?.ModuleID}");
        if (DnnModule == null) return l.ReturnAsOk("no module");

        // This changes site in whole scope
        DnnSite = ((DnnSite)_siteGenerator.New()).TryInitModule(DnnModule, Log);

        // New Context because Portal-Settings.Current is null
        var appId = module.BlockIdentifier.AppId;
        if (appId == AppConstants.AppIdNotFound || appId == Eav.Constants.NullId)
            return l.ReturnAsOk("no app id");

        // Ensure cache builds up with correct primary language
        // In case it's not loaded yet
        var appLoaderTool = _loaderTools.Value;
        _appsCache.Value.Load(module.BlockIdentifier, DnnSite.DefaultCultureCode, appLoaderTool);

        Block = _moduleAndBlockBuilder.Value.GetProvider(DnnModule, null).LoadBlock();

        if (Block.View == null) return "no view";
        if (Block.View.SearchIndexingDisabled) return "search disabled"; // new in 12.02

        // This list will hold all EAV entities to be indexed
        if (Block.Data == null) return "DataSource null";


        // Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
        AttachDnnLookUpsToData(Block.Data, DnnSite, DnnModule);

        // Get all streams to index
        var streamsToIndex = GetStreamsToIndex();
        if (!streamsToIndex.Any()) return l.ReturnAsOk("no streams to index");

        // Figure out the current edition - if none, stop here
        // New 2023-03-20 - if the view comes with a preset edition, it's an ajax-preview which should be respected
        _edition = PolymorphConfigReader.UseViewEditionLazyGetEdition(Block.View, () => _polymorphism.Init(Block.Context.AppState.List));
        //Block.View.Edition.NullIfNoValue()
        //           ?? _polymorphism.Init(Block.Context.AppState.List).Edition();

        // Convert DNN SearchDocuments from 2sxc SearchInfos
        SearchItems = BuildInitialSearchInfos(streamsToIndex, DnnModule);

        // all ok - return null so upstream knows no errors
        return l.ReturnNull();
    }

    /// <summary>The DnnModule will be initialized, and must exist for the search-index to provide data.</summary>
    public ModuleInfo DnnModule;
    /// <summary>The DnnSite will be initialized, and must exist for the search-index to provide data.</summary>
    public DnnSite DnnSite;
    /// <summary>The Block will be initialized, and must exist for the search-index to provide data.</summary>
    public IBlock Block;
    /// <summary>The SearchItems will be initialized, and must exist for the search-index to provide data.</summary>
    public Dictionary<string, List<ISearchItem>> SearchItems;

    private string _edition = default;

    /// <summary>
    /// Get search info for each dnn module containing 2sxc data
    /// </summary>
    /// <returns></returns>
    public IList<SearchDocument> GetModifiedSearchDocuments(IModule module, DateTime beginDate)
    {
        var l = Log.Fn<IList<SearchDocument>>();
        // Turn off logging into history by default - the template code can reactivate this if desired
        var logWithPreserve = Log as Log;
        if (logWithPreserve != null) logWithPreserve.Preserve = false;

        // Log with infos, to ensure errors are caught
        var exitMessage = InitAllAndVerifyIfOk(module);
        if (exitMessage != null)
            return l.Return(new List<SearchDocument>(), exitMessage);

        try
        {
            var useCustomViewController = !string.IsNullOrWhiteSpace(Block.View.ViewController); // new in 12.02
            l.A($"Use new Custom View Controller: {useCustomViewController}");
            if (useCustomViewController)
            {
                /* New mode in 12.02 using a custom ViewController */
                var customizeSearch = CreateAndInitViewController(DnnSite, Block);
                if (customizeSearch == null) return l.Return(new List<SearchDocument>(), "exit");

                // Call CustomizeSearch in a try/catch
                l.A("execute CustomizeSearch");
                customizeSearch.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
                l.A("Executed CustomizeSearch");
            }
            else
            {
                /* Old mode v06.02 - 12.01 using the Engine or Razor which customizes */
                // Build the engine, as that's responsible for calling inner search stuff
                var engine = _engineFactory.CreateEngine(Block.View);
                if (engine is IEngineDnnOldCompatibility oldEngine)
                {
                    oldEngine.Init(Block, Purpose.IndexingForSearch);

#pragma warning disable CS0618
                    // Only run CustomizeData() if we're in the older, classic model of search-indexing
                    // The new model v12.02 won't need this
                    l.A("Will run CustomizeData() in the Razor Engine which will call it in the Razor if exists");
                    oldEngine.CustomizeData();

                    // check if the cshtml has search customizations
                    l.A("Will run CustomizeSearch() in the Razor Engine which will call it in the Razor if exists");
                    oldEngine.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
#pragma warning restore CS0618
                } else
                    engine.Init(Block);

            }
        }
        catch (Exception e)
        {
            return l.Return(LogErrorForExit(e, DnnModule),
                "error, so return nothing to ensure we don't bleed unexpected infos");
        }

        // At the of the code, add it to insights / history. This must happen at the end.
        // It will only be preserved, if the inner code ran a Log.Preserve = true;
        if (logWithPreserve?.Preserve ?? false)
            _logStore.Value.Add("dnn-search", Log);

        // reduce load by only keeping recently modified items
        var searchDocuments = KeepOnlyChangesSinceLastIndex(beginDate, SearchItems);

        return l.Return(searchDocuments, $"{searchDocuments.Count}");
    }

        
        
    private List<SearchDocument> LogErrorForExit(Exception e, ModuleInfo modInfo)
    {
        DnnEnvironmentLogger.AddSearchExceptionToLog(modInfo, e, nameof(SearchController));
        Log.Ex(e);
        return new List<SearchDocument>();
    }
        
        

    /// <summary>
    /// Reduce load by only keeping recently modified items
    /// </summary>
    /// <param name="beginDate"></param>
    /// <param name="searchInfoDictionary"></param>
    /// <returns></returns>
    private static List<SearchDocument> KeepOnlyChangesSinceLastIndex(DateTime beginDate, Dictionary<string, List<ISearchItem>> searchInfoDictionary)
    {
        var searchDocuments = new List<SearchDocument>();
        foreach (var searchInfoList in searchInfoDictionary)
        {
            // Filter by Date - take only SearchDocuments that changed since beginDate
            var searchDocumentsToAdd = searchInfoList.Value.Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime())
                .Select(p => (SearchDocument) p);
            searchDocuments.AddRange(searchDocumentsToAdd);
        }

        return searchDocuments;
    }


    /// <summary>
    /// Convert DNN SearchDocuments from 2sxc SearchInfos
    /// </summary>
    private Dictionary<string, List<ISearchItem>> BuildInitialSearchInfos(KeyValuePair<string, IDataStream>[] streamsToIndex, ModuleInfo dnnModule)
    {
        var l = Log.Fn<Dictionary<string, List<ISearchItem>>>();
        var language = dnnModule.CultureCode;
        var searchInfoDictionary = new Dictionary<string, List<ISearchItem>>();
        foreach (var stream in streamsToIndex)
        {
            var entities = stream.Value.List.ToImmutableList();
            var searchInfoList = searchInfoDictionary[stream.Key] = new List<ISearchItem>();

            searchInfoList.AddRange(entities.Select(entity =>
            {
                var searchInfo = new SearchItem
                {
                    Entity = entity,
                    Url = "",
                    Description = "",
                    Body = GetJoinedAttributes(entity, language),
                    Title = entity.Title?[language]?.ToString() ?? "(no title)",
                    ModifiedTimeUtc = (entity.Modified == DateTime.MinValue
                        ? DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                        : entity.Modified).ToUniversalTime(),
                    UniqueKey = "2sxc-" + dnnModule.ModuleID + "-" + (entity.EntityGuid != new Guid()
                        ? entity.EntityGuid.ToString()
                        : stream.Key + "-" + entity.EntityId),
                    IsActive = true,
                    TabId = dnnModule.TabID,
                    PortalId = dnnModule.PortalID
                };

                return searchInfo;
            }));
        }

        return l.Return(searchInfoDictionary, $"{searchInfoDictionary.Count}");
    }

    /// <summary>
    /// Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
    /// </summary>
    private void AttachDnnLookUpsToData(IDataSource dataSource, DnnSite site, ModuleInfo dnnModule)
    {
        if (dataSource.Configuration?.LookUpEngine != null)
        {
            Log.A("Will try to attach dnn providers to DataSource LookUps");
            try
            {
                var getLookups = _dnnLookUpEngineResolver.Value;
                var dnnLookUps = (getLookups as DnnLookUpEngineResolver)?.GenerateDnnBasedLookupEngine(site.GetContents(), dnnModule.ModuleID);
                ((LookUpEngine) dataSource.Configuration.LookUpEngine).Link(dnnLookUps);
            }
            catch (Exception e)
            {
                // Log but keep going, as it's bad, but the lookups may not be important for this module
                Log.Ex(e);
            }
        }
    }

    /// <summary>
    /// Get original streams and if the settings restrict which ones to keep, apply that. 
    /// </summary>
    private KeyValuePair<string, IDataStream>[] GetStreamsToIndex()
    {
        var l = Log.Fn<KeyValuePair<string, IDataStream>[]>();
        // Check if we should filter the streams - new in 12.02
        var streamsToKeep = Block.View.SearchIndexingStreams
            .Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();

        // Decide what streams to keep - new in 12.02
        var streamsToIndex = Block.Data.Out
            .Where(p => p.Key != ViewParts.Presentation && p.Key != ViewParts.ListPresentation)
            .ToArray();

        if (streamsToKeep.Any())
            streamsToIndex = streamsToIndex
                .Where(s => streamsToKeep.Contains(s.Key, InvariantCultureIgnoreCase))
                .ToArray();

        return l.Return(streamsToIndex, $"{streamsToIndex.Length}");
    }

    private ICustomizeSearch CreateAndInitViewController(ISite site, IBlock block)
    {
        var l = Log.Fn<ICustomizeSearch>();
        // 1. Get and compile the view.ViewController
        var path = Path
            .Combine(Block.View.IsShared ? site.SharedAppsRootRelative() : site.AppsRootPhysical, block.Context.AppState.Folder)
            .ForwardSlash();
        l.A($"compile ViewController class on path: {path}/{Block.View.ViewController}");
        var spec = new HotBuildSpec { AppId = block.AppId, Edition = _edition };
        var instance = _codeCompiler.New().InstantiateClass(virtualPath: block.View.ViewController, spec: spec, className: null, relativePath: path, throwOnError: true);
        l.A("got instance of compiled ViewController class");

        // 2. Check if it implements ToSic.Sxc.Search.ICustomizeSearch - otherwise just return the empty search results as shown above
        if (!(instance is ICustomizeSearch customizeSearch)) return l.ReturnNull("exit, class do not implements ICustomizeSearch");

        // 3. Make sure it has the full context if it's based on DynamicCode (like Code12)
        if (instance is INeedsDynamicCodeRoot instanceWithContext)
        {
            l.A($"attach DynamicCode context to class instance");
            var parentDynamicCodeRoot = _codeRootFactory.New()
                .BuildCodeRoot(null, block, Log, CompatibilityLevels.CompatibilityLevel10);
            instanceWithContext.ConnectToRoot(parentDynamicCodeRoot);
        }

        return l.Return(customizeSearch, "instance ok");
    }

    private string StripHtmlAndHtmlDecode(string text) => HttpUtility.HtmlDecode(Regex.Replace(text, "<.*?>", string.Empty));

    /// <summary>
    /// Gets a string that represents all entities joined with a comma , separator
    /// Does just include String and Number fields
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    private string GetJoinedAttributes(IEntity entity, string language)
    {
        return string.Join(", ",
            entity.Attributes
                .Where(x => x.Value.Type == ValueTypes.String || x.Value.Type == ValueTypes.Number)
                .Select(x => x.Value[language])
                .Where(a => a != null)
                .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                .Where(x => !string.IsNullOrEmpty(x))) + " ";
    }
}