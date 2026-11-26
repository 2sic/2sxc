using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sxc.Search;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Dnn.Search;

/// <summary>
/// This will construct data for the search indexer in DNN.
/// It's created once for each module which will be indexed
/// </summary>
/// <remarks>
/// DNN context and documents stay here; search preparation is shared with Oqtane.
/// </remarks>
internal class SearchController(
    IAppsCacheSelector appsCache,
    LazySvc<ToSic.Sxc.Search.Sys.SearchIndexing> searchIndexing,
    LazySvc<DnnModuleContextFactory> ctxOfModFac,
    LazySvc<BlockOfModule> moduleAndBlockBuilder,
    LazySvc<ILookUpEngineResolver> dnnLookUpEngineResolver,
    LazySvc<ILogStore> logStore,
    IEditionService editionSvc)
    : ServiceBase("DNN.Search",
        connect: [appsCache, searchIndexing, ctxOfModFac, dnnLookUpEngineResolver, moduleAndBlockBuilder, logStore, editionSvc])
{
    /// <summary>
    /// Initialize all values which are needed - or return a text with the info why we must stop.
    /// For example, check if the block is in a ready state to provide data.
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    private string InitAllAndVerifyIfOk(IModule module)
    {
        var l = Log.Fn<string>();
        // Start by getting the module info
        DnnModule = (module as IWrapper<ModuleInfo>)?.GetContents();
        l.A($"start search for mod#{DnnModule?.ModuleID}");
        if (DnnModule == null)
            return l.ReturnAsOk("no module");

        // This changes site in whole scope
        DnnSite = ctxOfModFac.Value.CreateFromModuleAndShare(DnnModule, Log);

        // New Context because Portal-Settings.Current is null
        var identity = module.BlockIdentifier;
        if (identity == null || identity.AppId is AppConstants.AppIdNotFound or EavConstants.NullId or KnownAppsConstants.AppIdEmpty)
            return l.ReturnAsOk("no app id");

        // Ensure cache builds up with correct primary language in case it's not loaded yet.
        // We must access the cache early on with the injected culture,
        // to ensure that the loading from DB uses the correct culture for the primary language.
        var injectCulture = new ZoneCultureInfo(DnnSite.DefaultCultureCode, DnnSite.DefaultCultureCode);
        using (Ctx<IZoneCultureResolver>.Use(injectCulture))
            appsCache.Value.Get(identity.PureIdentity());

        var modCtx = ctxOfModFac.Value.InitDnnSiteModuleAndBlockContext(DnnModule);
        Block = moduleAndBlockBuilder.Value.GetBlockOfModule(modCtx);

        if (!Block.DataIsReady || !Block.ViewIsReady)
            return "no data or view";

        if (Block.View.SearchIndexingDisabled)
            return "search disabled"; // new in 12.02


        // Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
        AttachDnnLookUpsToData(Block.Data, DnnSite, DnnModule);

        // Get all streams to index
        var streamsToIndex = GetStreamsToIndex();
        if (!streamsToIndex.Any())
            return l.ReturnAsOk("no streams to index");

        // Figure out the current edition - if none, stop here
        // New 2023-03-20 - if the view comes with a preset edition, it's an ajax-preview which should be respected
        _edition = editionSvc.Edition(Block);

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

    private string _edition;

    /// <summary>
    /// Get search info for each dnn module containing 2sxc data
    /// </summary>
    /// <returns></returns>
    public IList<SearchDocument> GetModifiedSearchDocuments(IModule module, DateTime beginDate)
    {
        var l = Log.Fn<List<SearchDocument>>();
        // Turn off logging into history by default - the template code can reactivate this if desired
        var logWithPreserve = Log as Log;
        logWithPreserve?.Preserve = false;

        // Log with infos, to ensure errors are caught
        var exitMessage = InitAllAndVerifyIfOk(module);
        if (exitMessage != null)
            return l.Return([], exitMessage);

        var view = Block.View!;
        try
        {
            var useCustomViewController = !string.IsNullOrWhiteSpace(view.ViewController); // new in 12.02
            l.A($"Use new Custom View Controller: {useCustomViewController}");
            if (useCustomViewController)
            {
                /* New mode in 12.02 using a custom ViewController */
                var customizeSearch = CreateAndInitViewController(DnnSite, Block);
                if (customizeSearch == null)
                    return l.Return([], "exit");

                // Call CustomizeSearch in a try/catch
                l.A("execute CustomizeSearch");
                customizeSearch.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
                l.A("Executed CustomizeSearch");
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
            logStore.Value.Add("dnn-search", Log);

        // reduce load by only keeping recently modified items
        var searchDocuments = KeepOnlyChangesSinceLastIndex(beginDate, SearchItems);

        return l.Return(searchDocuments, $"{searchDocuments.Count}");
    }

        
        
    private List<SearchDocument> LogErrorForExit(Exception e, ModuleInfo modInfo)
    {
        DnnEnvironmentLogger.AddSearchExceptionToLog(modInfo, e, nameof(SearchController));
        Log.Ex(e);
        return [];
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
            var searchDocumentsToAdd = searchInfoList.Value
                .Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime())
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
            var entities = stream.Value.List.ToListOpt();
            var searchInfoList = searchInfoDictionary[stream.Key] = [];

            var additions = entities
                .Select(entity => new SearchItem
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
                })
                .ToListOpt();

            searchInfoList.AddRange(additions);
        }

        return l.Return(searchInfoDictionary, $"{searchInfoDictionary.Count}");
    }

    /// <summary>
    /// Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
    /// </summary>
    private void AttachDnnLookUpsToData(IDataSource dataSource, DnnSite site, ModuleInfo dnnModule)
    {
        if (dataSource.Configuration?.LookUpEngine == null)
            return;

        Log.A("Will try to attach dnn providers to DataSource LookUps");
        try
        {
            var getLookups = dnnLookUpEngineResolver.Value;
            var dnnLookUps = (getLookups as DnnLookUpEngineResolver)?.LookUpEngineOfPortalSettings(site.GetContents(), dnnModule.ModuleID);
            ((LookUpEngine) dataSource.Configuration.LookUpEngine).Link(dnnLookUps);
        }
        catch (Exception e)
        {
            // Log but keep going, as it's bad, but the lookups may not be important for this module
            Log.Ex(e);
        }
    }

    /// <summary>
    /// Get original streams and if the settings restrict which ones to keep, apply that. 
    /// </summary>
    private KeyValuePair<string, IDataStream>[] GetStreamsToIndex()
    {
        var l = Log.Fn<KeyValuePair<string, IDataStream>[]>();
        var streams = ToSic.Sxc.Search.Sys.SearchIndexing.Streams(Block.Data.Out, Block.View.SearchIndexingStreams);
        return l.Return(streams, $"{streams.Length}");
    }

    private ICustomizeSearch CreateAndInitViewController(ISite site, IBlock block)
        => searchIndexing.Value.CreateViewController(block, _edition,
            Path.Combine(block.View.IsShared ? site.SharedAppsRootRelative() : site.AppsRootPhysical,
                block.Context.AppReaderRequired.Specs.Folder).ForwardSlash());

    private string GetJoinedAttributes(IEntity entity, string language)
        => ToSic.Sxc.Search.Sys.SearchIndexing.Body(entity, language);
}
