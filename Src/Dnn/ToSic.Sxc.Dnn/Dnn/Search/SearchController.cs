using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Engines;
using static System.StringComparer;
using DynamicCode = ToSic.Sxc.Code.DynamicCode;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Search
{
    /// <summary>
    /// This will construct data for the search indexer in DNN.
    /// It's created once for each module which will be indexed
    /// </summary>
    internal class SearchController : HasLog
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchController(IServiceProvider serviceProvider, ILog parentLog) : base("DNN.Search", parentLog)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initialize all values which are needed - or return a text with the info why we must stop. 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        private string InitAllAndVerifyIfOk(IModule module)
        {
            // Start by getting the module info
            DnnModule = (module as Module<ModuleInfo>)?.UnwrappedContents;
            var wrapLog = Log.Call<string>($"start search for mod#{DnnModule?.ModuleID}");
            if (DnnModule == null) return wrapLog("cancel", "no module");
            
            // New Context because Portal-Settings.Current is null
            var appId = module.BlockIdentifier.AppId;
            if (appId == AppConstants.AppIdNotFound || appId == Eav.Constants.NullId) return wrapLog("cancel", "no app id");
            
            DnnSite = new DnnSite().TrySwap(DnnModule);

            // Ensure cache builds up with correct primary language
            // In case it's not loaded yet
            State.Cache.Load(module.BlockIdentifier, DnnSite.DefaultCultureCode);

            var dnnContext = Eav.Factory.StaticBuild<IContextOfBlock>().Init(DnnModule, Log);
            Block = _serviceProvider.Build<BlockFromModule>().Init(dnnContext, Log);
            
            if (Block.View == null) return wrapLog("cancel", "no view");
            if (Block.View.SearchIndexingDisabled) return wrapLog("cancel", "search disabled"); // new in 12.02

            // This list will hold all EAV entities to be indexed
            if (Block.Data == null) return wrapLog("cancel", "DataSource null");


            // Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
            AttachDnnLookUpsToData(Block.Data, DnnSite, DnnModule);

            // Get all streams to index
            var streamsToIndex = GetStreamsToIndex();
            if (!streamsToIndex.Any()) return wrapLog("cancel", "no streams to index");


            // Convert DNN SearchDocuments from 2sxc SearchInfos
            SearchItems = BuildInitialSearchInfos(streamsToIndex, DnnModule);

            // all ok
            return wrapLog("ok", null);
        }

        /// <summary>The DnnModule will be initialized, and must exist for the search-index to provide data.</summary>
        public ModuleInfo DnnModule;
        /// <summary>The DnnSite will be initialized, and must exist for the search-index to provide data.</summary>
        public DnnSite DnnSite;
        /// <summary>The Block will be initialized, and must exist for the search-index to provide data.</summary>
        public BlockFromModule Block;
        /// <summary>The SearchItems will be initialized, and must exist for the search-index to provide data.</summary>
        public Dictionary<string, List<ISearchItem>> SearchItems;

        /// <summary>
        /// Get search info for each dnn module containing 2sxc data
        /// </summary>
        /// <returns></returns>
        public IList<SearchDocument> GetModifiedSearchDocuments(IModule module, DateTime beginDate)
        {
            // Turn off logging into history by default - the template code can reactivate this if desired
            Log.Preserve = false;
            
            // Log with infos, to ensure errors are caught
            var wrapLog = Log.Call<IList<SearchDocument>>();
            var exitMessage = InitAllAndVerifyIfOk(module);
            if (exitMessage != null) 
                return wrapLog(exitMessage, new List<SearchDocument>());

            try
            {
                var useCustomViewController = !string.IsNullOrWhiteSpace(Block.View.ViewController); // new in 12.02
                Log.Add($"Use new Custom View Controller: {useCustomViewController}");
                if (useCustomViewController)
                {
                    /* New mode in 12.02 using a custom ViewController */
                    var customizeSearch = CreateAndInitViewController(DnnSite, Block);
                    if (customizeSearch == null) return wrapLog("exit", new List<SearchDocument>());

                    // Call CustomizeSearch in a try/catch
                    Log.Add("execute CustomizeSearch");
                    customizeSearch.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
                    Log.Add("Executed CustomizeSearch");
                }
                else
                {
                    /* Old mode v06.02 - 12.01 using the Engine or Razor which customizes */
                    // Build the engine, as that's responsible for calling inner search stuff
                    var engine = EngineFactory.CreateEngine(Block.View);
                    engine.Init(Block, Purpose.IndexingForSearch, Log);
                    
                    // Only run CustomizeData() if we're in the older, classic model of search-indexing
                    // The new model v12.02 won't need this
                    Log.Add("Will run CustomizeData() in the Razor Engine which will call it in the Razor if exists");
                    engine.CustomizeData();
                    
                    // check if the cshtml has search customizations
                    Log.Add("Will run CustomizeSearch() in the Razor Engine which will call it in the Razor if exists");
                    engine.CustomizeSearch(SearchItems, Block.Context.Module, beginDate);
                    // @STV: this was the old code, probably not needed, del if all is ok - previously it was
                    // engine.CustomizeSearch(searchInfoDictionary, _serviceProvider.Build<DnnModule>().Init(dnnModule, Log), beginDate);
                }
            }
            catch (Exception e)
            {
                return wrapLog("error, so return nothing to ensure we don't bleed unexpected infos", LogErrorForExit(e, DnnModule));
            }

            // At the of the code, add it to insights / history. This must happen at the end.
            // It will only be preserved, if the inner code ran a Log.Preserve = true;
            History.Add("dnn-search", Log);

            // reduce load by only keeping recently modified items
            var searchDocuments = KeepOnlyChangesSinceLastIndex(beginDate, SearchItems);

            return wrapLog($"{searchDocuments.Count}", searchDocuments);
        }

        
        
        private List<SearchDocument> LogErrorForExit(Exception e, ModuleInfo modInfo)
        {
            DnnBusinessController.AddSearchExceptionToLog(modInfo, e, nameof(SearchController));
            Log.Exception(e);
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
            var wrapLog = Log.Call<Dictionary<string, List<ISearchItem>>>();
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

            return wrapLog($"{searchInfoDictionary.Count}", searchInfoDictionary);
        }

        /// <summary>
        /// Attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
        /// </summary>
        private void AttachDnnLookUpsToData(IDataSource dataSource, DnnSite site, ModuleInfo dnnModule)
        {
            if (dataSource.Configuration?.LookUpEngine != null)
            {
                Log.Add("Will try to attach dnn providers to DataSource LookUps");
                try
                {
                    var getLookups = (DnnLookUpEngineResolver) _serviceProvider.Build<DnnLookUpEngineResolver>().Init(Log);
                    var dnnLookUps = getLookups.GenerateDnnBasedLookupEngine(site.UnwrappedContents, dnnModule.ModuleID);
                    ((LookUpEngine) dataSource.Configuration.LookUpEngine).Link(dnnLookUps);
                }
                catch (Exception e)
                {
                    // Log but keep going, as it's bad, but the lookups may not be important for this module
                    Log.Exception(e);
                }
            }
        }

        /// <summary>
        /// Get original streams and if the settings restrict which ones to keep, apply that. 
        /// </summary>
        private KeyValuePair<string, IDataStream>[] GetStreamsToIndex()
        {
            var wrapLog = Log.Call<KeyValuePair<string, IDataStream>[]>();
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
            
            return wrapLog($"{streamsToIndex.Length}", streamsToIndex);
        }

        private ICustomizeSearch CreateAndInitViewController(DnnSite site, BlockFromModule block)
        {
            var wrapLog = Log.Call<ICustomizeSearch>();
            // 1. Get and compile the view.ViewController
            var codeCompiler = _serviceProvider.Build<CodeCompiler>();
            var path = Path
                .Combine(Block.View.IsShared ? site.SharedAppsRootRelative : site.AppsRootRelative, block.Context.AppState.Folder)
                .ForwardSlash();
            Log.Add($"compile ViewController class on path: {path}/{Block.View.ViewController}");
            var instance = codeCompiler.InstantiateClass(block.View.ViewController, null, path, true);
            Log.Add("got instance of compiled ViewController class");

            // 2. Check if it implements ToSic.Sxc.Search.ICustomizeSearch - otherwise just return the empty search results as shown above
            if (!(instance is ICustomizeSearch customizeSearch)) return wrapLog("exit, class do not implements ICustomizeSearch", null);

            // 3. Make sure it has the full context if it's based on DynamicCode (like Code12)
            if (instance is DynamicCode instanceWithContext)
            {
                Log.Add($"attach DynamicCode context to class instance");
                var parentDynamicCodeRoot = _serviceProvider.Build<DnnDynamicCodeRoot>().Init(block, Log);
                instanceWithContext.DynamicCodeCoupling(parentDynamicCodeRoot);
            }

            return wrapLog("instance ok", customizeSearch);
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
                entity.Attributes.Where(x => x.Value.Type == DataTypes.String || x.Value.Type == DataTypes.Number).Select(x => x.Value[language])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}