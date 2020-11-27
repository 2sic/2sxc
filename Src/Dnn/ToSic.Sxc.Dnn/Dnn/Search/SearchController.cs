using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Engines;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Search
{
    internal class SearchController: HasLog
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
            var dnnModule = (module as Container<ModuleInfo>)?.UnwrappedContents;
            // always log with method, to ensure errors are caught
            Log.Add($"start search for mod#{dnnModule?.ModuleID}");

            // turn off logging into history by default - the template code can reactivate this if desired
            Log.Preserve = false;

            if (dnnModule == null) return searchDocuments;

            // New Context because Portal-Settings.Current is null
            var appId = module.BlockIdentifier.AppId;

            if (appId == AppConstants.AppIdNotFound || appId == Eav.Constants.NullId) return searchDocuments;

            // Since Portal-Settings.Current is null, instantiate with modules' portal id (which can be a different portal!)
            //var portalSettings = new PortalSettings(dnnModule.OwnerPortalID);
            var site = new DnnSite().TrySwap(dnnModule);//.Swap(portalSettings);

            // Ensure cache builds up with correct primary language
            var cache = State.Cache;
            cache.Load(module.BlockIdentifier, site.DefaultLanguage);

            var dnnContext = Eav.Factory.StaticBuild<IContextOfBlock>().Init(dnnModule, Log);
            var modBlock = _serviceProvider.Build<BlockFromModule>()
                .Init(dnnContext, Log);
                //.Init(DnnContextOfBlock.Create(site, container, Eav.Factory.GetServiceProvider()), Log);

            var language = dnnModule.CultureCode;

            var view = modBlock.View;

            if (view == null) return searchDocuments;

            // This list will hold all EAV entities to be indexed
            var dataSource = modBlock.Data;

            // 2020-03-12 Try to attach DNN Lookup Providers so query-params like [DateTime:Now] or [Portal:PortalId] will work
            if (dataSource?.Configuration?.LookUps != null)
            {
                Log.Add("Will try to attach dnn providers to DataSource LookUps");
                try
                {
                    var getLookups = (GetDnnEngine)_serviceProvider.Build<GetDnnEngine>().Init(Log);
                    var dnnLookUps = getLookups.GenerateDnnBasedLookupEngine(site.UnwrappedContents, dnnModule.ModuleID);
                    ((LookUpEngine) dataSource.Configuration.LookUps).Link(dnnLookUps);
                }
                catch(Exception e)
                {
                    Log.Add("Ran into an issue with an error: " + e.Message);
                }
            }


            var engine = EngineFactory.CreateEngine(view);
            engine.Init(modBlock, Purpose.IndexingForSearch, Log);

            // see if data customization inside the cshtml works
            try
            {
                engine.CustomizeData();
            }
            catch (Exception e) // Catch errors here, because of references to Request etc.
            {
                Exceptions.LogException(new SearchIndexException(dnnModule, e));
            }

            var searchInfoDictionary = new Dictionary<string, List<ISearchItem>>();

            // Get DNN SearchDocuments from 2Sexy SearchInfos
            foreach (var stream in dataSource.Out.Where(p => p.Key != ViewParts.Presentation && p.Key != ViewParts.ListPresentation))
            {
                
                var entities = stream.Value.Immutable;
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

            // check if the cshtml has search customizations
            try
            {
                engine.CustomizeSearch(searchInfoDictionary, 
                    _serviceProvider.Build<DnnModule>().Init(dnnModule, Log), beginDate);
            }
            catch (Exception e)
            {
                Exceptions.LogException(new SearchIndexException(dnnModule, e));
            }

            // add it to insights / history. It will only be preserved, if the inner code ran a Log.Preserve = true;
            History.Add("dnn-search", Log);

            // reduce load by only keeping recently modified items
            foreach (var searchInfoList in searchInfoDictionary)
            {
                // Filter by Date - take only SearchDocuments that changed since beginDate
                var searchDocumentsToAdd = searchInfoList.Value.Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime()).Select(p => (SearchDocument) p);
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
                entity.Attributes.Where(x => x.Value.Type == Eav.Constants.DataTypeString || x.Value.Type == Eav.Constants.DataTypeNumber).Select(x => x.Value[language])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}