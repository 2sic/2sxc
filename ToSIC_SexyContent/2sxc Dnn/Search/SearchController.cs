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
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Search;
using ToSic.Eav.DataSources.Caches;
using ToSic.Sxc.Interfaces;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7.Search
{
    internal class SearchController: HasLog
    {
        public SearchController(ILog parentLog = null) : base("DNN.Search", parentLog) { }

        /// <summary>
        /// Get search info for each dnn module containing 2sxc data
        /// </summary>
        /// <returns></returns>
        public IList<SearchDocument> GetModifiedSearchDocuments(IInstanceInfo instance, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();
            var dnnModule = (instance as EnvironmentInstance<ModuleInfo>)?.Original;
            // always log with method, to ensure errors are cought
            Log.Add($"start search for mod#{dnnModule?.ModuleID}");

            History.Add("dnn-search", Log);

            if (dnnModule == null) return searchDocuments;

            var isContentModule = dnnModule.DesktopModule.ModuleName == "2sxc";

            // New Context because PortalSettings.Current is null
            var zoneId = new DnnEnvironment(Log).ZoneMapper.GetZoneId(dnnModule.OwnerPortalID);

            var appId = !isContentModule
                ? new DnnMapAppToInstance(Log).GetAppIdFromInstance(instance, zoneId)
                : new ZoneRuntime(zoneId, Log).DefaultAppId;

            if (!appId.HasValue)
                return searchDocuments;

            // As PortalSettings.Current is null, instanciate with modules' portal id
            var portalSettings = new PortalSettings(dnnModule.OwnerPortalID);

            // Ensure cache builds up with correct primary language
            var cache = Eav.Factory.Resolve<ICache>();
            ((BaseCache)cache).ZoneId = zoneId;
            ((BaseCache)cache).AppId = appId.Value;
            cache.PreLoadCache(portalSettings.DefaultLanguage.ToLower());

            // must find tenant through module, as the PortalSettings.Current is null in search mode
            var tenant = new DnnTenant(portalSettings);
            var mcb = new ModuleContentBlock(instance, Log, tenant);
            var sexy = mcb.SxcInstance;

            var language = dnnModule.CultureCode;

            var contentGroup = sexy.App.ContentGroupManager.GetInstanceContentGroup(dnnModule.ModuleID, dnnModule.TabID);

            var template = contentGroup.Template;

            // This list will hold all EAV entities to be indexed
            var dataSource = sexy.Data;
			
            if (template == null)
                return searchDocuments;

            var engine = EngineFactory.CreateEngine(template);
            engine.Init(template, sexy.App, new DnnInstanceInfo(dnnModule), dataSource, InstancePurposes.IndexingForSearch, sexy, Log);

            // see if data customization inside the cshtml works
            try
            {
                engine.CustomizeData();
            }
            catch (Exception e) // Catch errors here, because of references to Request etc.
            {
                Exceptions.LogException(new SearchIndexException(dnnModule, e));
            }

            
            var searchInfoDictionary = new Dictionary<string, List<ISearchInfo>>();

            // Get DNN SearchDocuments from 2Sexy SearchInfos
            foreach (var stream in dataSource.Out.Where(p => p.Key != AppConstants.Presentation && p.Key != AppConstants.ListPresentation))
            {
                
                var entities = stream.Value.List;
                var searchInfoList = searchInfoDictionary[stream.Key] = new List<ISearchInfo>();

                searchInfoList.AddRange(entities.Select(entity =>
                {
                    var searchInfo = new SearchInfo
                    {
                        Entity = entity,
                        Url = "",
                        Description = "",
                        Body = GetJoinedAttributes(entity, language),
                        Title = entity.Title?[language]?.ToString() ?? "(no title)",
                        ModifiedTimeUtc = (entity.Modified == DateTime.MinValue ? DateTime.Now.Date.AddHours(DateTime.Now.Hour) : entity.Modified).ToUniversalTime(),
                        UniqueKey = "2sxc-" + dnnModule.ModuleID + "-" + (entity.EntityGuid != new Guid() ? entity.EntityGuid.ToString() : (stream.Key + "-" + entity.EntityId)),
                        IsActive = true,
                        TabId = dnnModule.TabID,
                        PortalId = dnnModule.PortalID
                    };

                    // Take the newest value (from ContentGroupItem and Entity)
                    if (entity is IHasEditingData typed)
                    {
                        var contentGroupItemModifiedUtc = typed.ContentGroupItemModified.ToUniversalTime();
                        searchInfo.ModifiedTimeUtc = searchInfo.ModifiedTimeUtc > contentGroupItemModifiedUtc
                            ? searchInfo.ModifiedTimeUtc
                            : contentGroupItemModifiedUtc;
                    }

                    return searchInfo;
                }));
            }

            // check if the cshtml has search customizations
            try
            {
                engine.CustomizeSearch(searchInfoDictionary, new DnnInstanceInfo(dnnModule), beginDate);
            }
            catch (Exception e)
            {
                Exceptions.LogException(new SearchIndexException(dnnModule, e));
            }

            // reduce load by only keeping recently modified ites
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
        private string GetJoinedAttributes(Eav.Interfaces.IEntity entity, string language)
        {
            return string.Join(", ",
                entity.Attributes.Where(x => x.Value.Type == "String" || x.Value.Type == "Number").Select(x => x.Value[language])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}