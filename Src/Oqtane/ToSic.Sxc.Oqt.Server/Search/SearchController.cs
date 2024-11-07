using Oqtane.Infrastructure;
using Oqtane.Models;
using System;
using System.Collections.Immutable;
using System.IO;
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
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.Search;
using static System.StringComparer;
using Log = ToSic.Lib.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Search;

/// <summary>
/// This will construct data for the search indexer in Oqtane.
/// It's created once for each module which will be indexed
/// </summary>
internal class SearchController(
    AppsCacheSwitch appsCache,
    Generator<CodeCompiler> codeCompiler,
    Generator<CodeApiServiceFactory> codeRootFactory,
    Generator<ISite> siteGenerator,
    LazySvc<IModuleAndBlockBuilder> moduleAndBlockBuilder,
    LazySvc<ILookUpEngineResolver> dnnLookUpEngineResolver,
    EngineFactory engineFactory,
    LazySvc<ILogStore> logStore,
    PolymorphConfigReader polymorphism,
    ILocalizationManager localizationManager)
    : ServiceBase("DNN.Search",
        connect:
        [
            appsCache, codeCompiler, codeRootFactory, siteGenerator, engineFactory, dnnLookUpEngineResolver,
            moduleAndBlockBuilder, logStore, polymorphism
        ])/*, ISearchController<SearchContent>*/
{
    /// <summary>
    /// Initialize all values which are needed - or return a text with the info why we must stop. 
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    private string InitAllAndVerifyIfOk(IModule module)
    {
        var l = Log.Fn<string>();
        // Start by getting the module info
        OqtModule = (module as OqtModule)?.GetContents();
        l.A($"start search for mod#{OqtModule?.ModuleId}");
        if (OqtModule == null) return l.ReturnAsOk("no module");

        // This changes site in whole scope
        OqtSite = (OqtSite)siteGenerator.New().Init(OqtModule.SiteId, Log);

        // New Context because Portal-Settings.Current is null
        var appId = module.BlockIdentifier.AppId;
        if (appId == AppConstants.AppIdNotFound || appId == Eav.Constants.NullId)
            return l.ReturnAsOk("no app id");

        // Ensure cache builds up with correct primary language
        // In case it's not loaded yet
        appsCache.Value.Load(module.BlockIdentifier.PureIdentity(), OqtSite.DefaultCultureCode, appsCache.AppLoaderTools);

        Block = moduleAndBlockBuilder.Value.BuildBlock(OqtModule, null);

        if (Block.View == null) return "no view";
        if (Block.View.SearchIndexingDisabled) return "search disabled"; // new in 12.02

        // This list will hold all EAV entities to be indexed
        if (Block.Data == null) return "DataSource null";


        //// Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
        //AttachDnnLookUpsToData(Block.Data, OqtSite, OqtModule);

        // Get all streams to index
        var streamsToIndex = GetStreamsToIndex();
        if (!streamsToIndex.Any()) return l.ReturnAsOk("no streams to index");

        // Figure out the current edition - if none, stop here
        // New 2023-03-20 - if the view comes with a preset edition, it's an ajax-preview which should be respected
        _edition = polymorphism.UseViewEditionOrGet(Block);
        //Block.View.Edition.NullIfNoValue()
        //           ?? _polymorphism.Init(Block.Context.AppState.List).Edition();

        // Convert DNN SearchDocuments from 2sxc SearchInfos
        SearchItems = BuildInitialSearchInfos(streamsToIndex, OqtModule);

        // all ok - return null so upstream knows no errors
        return l.ReturnNull();
    }

    /// <summary>The DnnModule will be initialized, and must exist for the search-index to provide data.</summary>
    public Module OqtModule;
    /// <summary>The DnnSite will be initialized, and must exist for the search-index to provide data.</summary>
    public OqtSite OqtSite;
    /// <summary>The Block will be initialized, and must exist for the search-index to provide data.</summary>
    public IBlock Block;
    /// <summary>The SearchItems will be initialized, and must exist for the search-index to provide data.</summary>
    public Dictionary<string, List<ISearchItem>> SearchItems;

    private string _edition = default;

    /// <summary>
    /// Get search info for each dnn module containing 2sxc data
    /// </summary>
    /// <returns></returns>
    public IList<SearchContent> GetModifiedSearchDocuments(IModule module, DateTime beginDate)
    {
        var l = Log.Fn<IList<SearchContent>>();
        // Turn off logging into history by default - the template code can reactivate this if desired
        var logWithPreserve = Log as Log;
        if (logWithPreserve != null) logWithPreserve.Preserve = false;

        // Log with infos, to ensure errors are caught
        var exitMessage = InitAllAndVerifyIfOk(module);
        if (exitMessage != null)
            return l.Return(new List<SearchContent>(), exitMessage);

        try
        {
            var useCustomViewController = !string.IsNullOrWhiteSpace(Block.View.ViewController); // new in 12.02
            l.A($"Use new Custom View Controller: {useCustomViewController}");
            if (useCustomViewController)
            {
                /* New mode in 12.02 using a custom ViewController */
                var customizeSearch = CreateAndInitViewController(OqtSite, Block);
                if (customizeSearch == null) return l.Return(new List<SearchContent>(), "exit");

                // Call CustomizeSearch in a try/catch
                l.A("execute CustomizeSearch");
                customizeSearch.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
                l.A("Executed CustomizeSearch");
            }
            else
            {
                /* Old mode v06.02 - 12.01 using the Engine or Razor which customizes */
                // Build the engine, as that's responsible for calling inner search stuff
                var engine = engineFactory.CreateEngine(Block.View);
                //                if (engine is IEngineDnnOldCompatibility oldEngine)
                //                {
                //#pragma warning disable CS0618
                //                    oldEngine.Init(Block, Purpose.IndexingForSearch);

                //                    // Only run CustomizeData() if we're in the older, classic model of search-indexing
                //                    // The new model v12.02 won't need this
                //                    l.A("Will run CustomizeData() in the Razor Engine which will call it in the Razor if exists");
                //                    oldEngine.CustomizeData();

                //                    // check if the cshtml has search customizations
                //                    l.A("Will run CustomizeSearch() in the Razor Engine which will call it in the Razor if exists");
                //                    oldEngine.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
                //#pragma warning restore CS0618
                //                } else
                engine.Init(Block);
            }
        }
        catch (Exception e)
        {
            return l.Return(LogErrorForExit(e, OqtModule),
                "error, so return nothing to ensure we don't bleed unexpected infos");
        }

        // At the of the code, add it to insights / history. This must happen at the end.
        // It will only be preserved, if the inner code ran a Log.Preserve = true;
        if (logWithPreserve?.Preserve ?? false)
            logStore.Value.Add("dnn-search", Log);

        // reduce load by only keeping recently modified items
        var searchDocuments = KeepOnlyChangesSinceLastIndex(beginDate, SearchItems);

        return l.Return(searchDocuments, $"{searchDocuments.Count}");
    }



    private List<SearchContent> LogErrorForExit(Exception e, Module oqtModule)
    {
        //DnnEnvironmentLogger.AddSearchExceptionToLog(oqtModule, e, nameof(SearchController));
        Log.Ex(e);
        return [];
    }



    /// <summary>
    /// Reduce load by only keeping recently modified items
    /// </summary>
    /// <param name="beginDate"></param>
    /// <param name="searchInfoDictionary"></param>
    /// <returns></returns>
    private static List<SearchContent> KeepOnlyChangesSinceLastIndex(DateTime beginDate, Dictionary<string, List<ISearchItem>> searchInfoDictionary)
    {
        var searchDocuments = new List<SearchContent>();
        foreach (var searchInfoList in searchInfoDictionary)
        {
            // Filter by Date - take only SearchDocuments that changed since beginDate
            var searchDocumentsToAdd = searchInfoList.Value.Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime())
                .Select(p => (SearchContent)p);
            searchDocuments.AddRange(searchDocumentsToAdd);
        }

        return searchDocuments;
    }


    /// <summary>
    /// Convert DNN SearchDocuments from 2sxc SearchInfos
    /// </summary>
    private Dictionary<string, List<ISearchItem>> BuildInitialSearchInfos(KeyValuePair<string, IDataStream>[] streamsToIndex, Module oqtModule)
    {
        var l = Log.Fn<Dictionary<string, List<ISearchItem>>>();
        var language = /*oqtModule.CultureCode*/localizationManager.GetDefaultCulture(); // TODO: @STV check for case when site support more languages
        var searchInfoDictionary = new Dictionary<string, List<ISearchItem>>();
        foreach (var stream in streamsToIndex)
        {
            var entities = stream.Value.List.ToImmutableList();
            var searchInfoList = searchInfoDictionary[stream.Key] = [];

            searchInfoList.AddRange(entities.Select(entity =>
            {
                var searchInfo = new SearchItem
                {
                    EntityName = entity.Type.Name,
                    EntityId = entity.EntityId.ToString(),
                    Title = entity.Title?[language]?.ToString() ?? "(no title)",
                    Description = "", // a summary description of the content (optional)
                    Body = GetJoinedAttributes(entity, language), // the actual content
                    Url = "", // the url for accessing the page where the content exists (starting with the page path - not including any alias)
                    ContentModifiedOn = entity.Modified == DateTime.MinValue ? oqtModule.ModifiedOn : entity.Modified,
                    AdditionalContent = string.Empty,
                    CreatedOn = entity.Created,

                    Entity = entity,
                    ModifiedTimeUtc = (entity.Modified == DateTime.MinValue ? oqtModule.ModifiedOn : entity.Modified).ToUniversalTime(),
                    UniqueKey = "2sxc-" + oqtModule.ModuleId + "-" + (entity.EntityGuid != Guid.Empty
                        ? entity.EntityGuid.ToString()
                        : stream.Key + "-" + entity.EntityId),
                    IsActive = true,
                    CultureCode = language,
                    QueryString = "" // TODO: @STV find how to get query string part
                };

                return searchInfo;
            }));
        }

        return l.Return(searchInfoDictionary, $"{searchInfoDictionary.Count}");
    }

    ///// <summary>
    ///// Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
    ///// </summary>
    //private void AttachDnnLookUpsToData(IDataSource dataSource, OqtSite site, Module<Module> oqtModule)
    //{
    //    if (dataSource.Configuration?.LookUpEngine != null)
    //    {
    //        Log.A("Will try to attach dnn providers to DataSource LookUps");
    //        try
    //        {
    //            var getLookups = dnnLookUpEngineResolver.Value;
    //            var dnnLookUps = (getLookups as DnnLookUpEngineResolver)?.LookUpEngineOfPortalSettings(site.GetContents(), oqtModule.Id);
    //            ((LookUpEngine) dataSource.Configuration.LookUpEngine).Link(dnnLookUps);
    //        }
    //        catch (Exception e)
    //        {
    //            // Log but keep going, as it's bad, but the lookups may not be important for this module
    //            Log.Ex(e);
    //        }
    //    }
    //}

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
            .Combine(/*TODO: @STV Block.View.IsShared ? site.SharedAppsRootRelative() :*/ site.AppsRootPhysical, block.Context.AppReader.Specs.Folder)
            .ForwardSlash();
        l.A($"compile ViewController class on path: {path}/{Block.View.ViewController}");
        var spec = new HotBuildSpec(block.AppId, _edition, block.App?.Name);
        l.A($"prepare spec: {spec}");
        var instance = codeCompiler.New().InstantiateClass(virtualPath: block.View.ViewController, spec: spec, className: null, relativePath: path, throwOnError: true);
        l.A("got instance of compiled ViewController class");

        // 2. Check if it implements ToSic.Sxc.Search.ICustomizeSearch - otherwise just return the empty search results as shown above
        if (!(instance is ICustomizeSearch customizeSearch)) return l.ReturnNull("exit, class do not implements ICustomizeSearch");

        // 3. Make sure it has the full context if it's based on DynamicCode (like Code12)
        if (instance is INeedsCodeApiService instanceWithContext)
        {
            l.A($"attach DynamicCode context to class instance");
            var parentDynamicCodeRoot = codeRootFactory.New()
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