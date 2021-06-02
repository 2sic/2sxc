using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
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
using ToSic.Sxc.Run;
using DynamicCode = ToSic.Sxc.Code.DynamicCode;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Search
{
    internal class SearchController : HasLog
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchController(IServiceProvider serviceProvider, ILog parentLog) : base("DNN.Search", parentLog)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get search info for each dnn module containing 2sxc data
        /// </summary>
        /// <returns></returns>
        public IList<SearchDocument> GetModifiedSearchDocuments(IModule module, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();
            var dnnModule = (module as Module<ModuleInfo>)?.UnwrappedContents;
            // always log with method, to ensure errors are caught
            Log.Add($"start search for mod#{dnnModule?.ModuleID}");

            // turn off logging into history by default - the template code can reactivate this if desired
            Log.Preserve = true; // TODO: WIP KEEP ACTIVE TILL V12.02 IMPLEMENTED

            if (dnnModule == null) return searchDocuments;

            // New Context because Portal-Settings.Current is null
            var appId = module.BlockIdentifier.AppId;

            if (appId == AppConstants.AppIdNotFound || appId == Eav.Constants.NullId) return searchDocuments;

            var site = new DnnSite().TrySwap(dnnModule);

            // Ensure cache builds up with correct primary language
            var cache = State.Cache;
            cache.Load(module.BlockIdentifier, site.DefaultCultureCode);

            var dnnContext = Eav.Factory.StaticBuild<IContextOfBlock>().Init(dnnModule, Log);
            var modBlock = _serviceProvider.Build<BlockFromModule>()
                .Init(dnnContext, Log);

            var language = dnnModule.CultureCode;

            var view = modBlock.View;

            if (view == null) return searchDocuments;
            if (view.SearchIndexingDisabled) return searchDocuments; // new in 12.02
            var useCustomViewController = !string.IsNullOrWhiteSpace(view.ViewController); // new in 12.02

            // This list will hold all EAV entities to be indexed
            var dataSource = modBlock.Data;

            // 2020-03-12 Try to attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
            if (dataSource?.Configuration?.LookUpEngine != null)
            {
                Log.Add("Will try to attach dnn providers to DataSource LookUps");
                try
                {
                    var getLookups = (DnnLookUpEngineResolver)_serviceProvider.Build<DnnLookUpEngineResolver>().Init(Log);
                    var dnnLookUps = getLookups.GenerateDnnBasedLookupEngine(site.UnwrappedContents, dnnModule.ModuleID);
                    ((LookUpEngine)dataSource.Configuration.LookUpEngine).Link(dnnLookUps);
                }
                catch (Exception e)
                {
                    Log.Add("Ran into an issue with an error: " + e.Message);
                }
            }

            // We only need the engine if we're in classic mode
            IEngine engine = null;

            // Only run CustomizeData() if we're in the older, classic model of search-indexing
            // The new model v12.02 won't need this
            if (!useCustomViewController)
            {
                engine = EngineFactory.CreateEngine(view);
                engine.Init(modBlock, Purpose.IndexingForSearch, Log);
                // see if data customization inside the cshtml works
                try
                {
                    engine.CustomizeData();
                }
                catch (Exception e) // Catch errors here, because of references to Request etc.
                {
                    DnnBusinessController.AddSearchExceptionToLog(dnnModule, e, nameof(SearchController));
                }
            }

            var searchInfoDictionary = new Dictionary<string, List<ISearchItem>>();

            // Get DNN SearchDocuments from 2Sexy SearchInfos
            foreach (var stream in dataSource.Out.Where(p => p.Key != ViewParts.Presentation && p.Key != ViewParts.ListPresentation))
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
                        UniqueKey = "2sxc-" + dnnModule.ModuleID + "-" + (entity.EntityGuid != new Guid() ? entity.EntityGuid.ToString() : (stream.Key + "-" + entity.EntityId)),
                        IsActive = true,
                        TabId = dnnModule.TabID,
                        PortalId = dnnModule.PortalID
                    };

                    return searchInfo;
                }));
            }

            if (useCustomViewController)
            {
                var wrapLog = Log.Call<List<SearchDocument>>($"useCustomViewController: {useCustomViewController}");
                
                try
                {
                    // 1. Get and compile the view.ViewController
                    var codeCompiler = _serviceProvider.Build<CodeCompiler>();
                    var path = Path.Combine(site.AppsRootRelative, dnnContext.AppState.Folder).ForwardSlash();
                    Log.Add($"path: {path}/{view.ViewController}");
                    var instance = codeCompiler.InstantiateClass(view.ViewController, null, path, true);

                    // 2. Check if it implements ToSic.Sxc.Search.ICustomizeSearch - otherwise just return the empty search results as shown above
                    if (!(instance is ICustomizeSearch customizeSearch)) return wrapLog("exit, class do not implements implements ToSic.Sxc.Search.ICustomizeSearch", searchDocuments);

                    // 3. Make sure it has the full context if it's based on DynamicCode (like Code12)
                    if (instance is DynamicCode instanceWithContext)
                    {
                        Log.Add($"attach context");
                        var parentDynamicCodeRoot = _serviceProvider.Build<DnnDynamicCodeRoot>().Init(modBlock, Log);
                        instanceWithContext.DynamicCodeCoupling(parentDynamicCodeRoot);
                    }

                    // 4. Call CustomizeSearch in a try/catch
                    var dm = _serviceProvider.Build<DnnModule>().Init(dnnModule, Log);
                    Log.Add($"execute CustomizeSearch");
                    customizeSearch.CustomizeSearch(searchInfoDictionary, dm, beginDate);
                    wrapLog("OK, executed CustomizeSearch", null);
                }
                catch (Exception e)
                {
                    wrapLog($"Error: {e.Message}, {e.InnerException}", null);
                    DnnBusinessController.AddSearchExceptionToLog(dnnModule, e, nameof(SearchController));
                }
            }
            else
            {
                // check if the cshtml has search customizations
                try
                {
                    if (engine == null)
                        throw new Exception("engine==null in classic search. This should never happen.");
                    engine.CustomizeSearch(searchInfoDictionary,
                        _serviceProvider.Build<DnnModule>().Init(dnnModule, Log), beginDate);
                }
                catch (Exception e)
                {
                    DnnBusinessController.AddSearchExceptionToLog(dnnModule, e, nameof(SearchController));
                }
            }

            // add it to insights / history. It will only be preserved, if the inner code ran a Log.Preserve = true;
            History.Add("dnn-search", Log);

            // reduce load by only keeping recently modified items
            foreach (var searchInfoList in searchInfoDictionary)
            {
                // Filter by Date - take only SearchDocuments that changed since beginDate
                var searchDocumentsToAdd = searchInfoList.Value.Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime()).Select(p => (SearchDocument)p);
                searchDocuments.AddRange(searchDocumentsToAdd);
            }

            return searchDocuments;
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