using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Statics;

namespace ToSic.SexyContent.Search
{
    public class SearchController
    {

        public IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();

            var isContentModule = moduleInfo.DesktopModule.ModuleName == "2sxc";

            // New Context because PortalSettings.Current is null
            var zoneId = ZoneHelpers.GetZoneID(moduleInfo.OwnerPortalID);

            if (!zoneId.HasValue)
                return searchDocuments;

            int? appId = AppHelpers.GetDefaultAppId(zoneId.Value);

            if (!isContentModule)
            {
	            appId = AppHelpers.GetAppIdFromModule(moduleInfo);
				if (!appId.HasValue)
		            return searchDocuments;
            }

            var sexy = new InstanceContext(zoneId.Value, appId.Value, true, moduleInfo.OwnerPortalID, moduleInfo);
            var language = moduleInfo.CultureCode;
	        var contentGroup = sexy.ContentGroups.GetContentGroupForModule(moduleInfo.ModuleID);
            var template = contentGroup.Template;

            // This list will hold all EAV entities to be indexed
            // before 2016-02-27 2dm: var dataSource = sexy.GetViewDataSource(moduleInfo.ModuleID, false, template);
            var dataSource = sexy.DataSource;// ViewDataSource.ForModule(moduleInfo.ModuleID, false, template, sexy);
			
            if (template == null)
                return searchDocuments;

            var engine = EngineFactory.CreateEngine(template);
            engine.Init(template, sexy.App, moduleInfo, dataSource, InstancePurposes.IndexingForSearch, sexy);

            try
            {
                engine.CustomizeData();
            }
            catch (Exception e) // Catch errors here, because of references to Request etc.
            {
                Exceptions.LogException(new SearchIndexException(moduleInfo, e));
            }

            
            var searchInfoDictionary = new Dictionary<string, List<ISearchInfo>>();

            // Get DNN SearchDocuments from 2Sexy SearchInfos
            foreach (var stream in dataSource.Out.Where(p => p.Key != "Presentation" && p.Key != "ListPresentation" ))
            {
                
                var entities = stream.Value.List.Select(p => p.Value);
                var searchInfoList = searchInfoDictionary[stream.Key] = new List<ISearchInfo>();

                searchInfoList.AddRange(entities.Select(entity =>
                {
                    var searchInfo = new SearchInfo
                    {
                        Entity = entity,
                        Url = "",
                        Description = "",
                        Body = GetJoinedAttributes(entity, language),
                        Title = entity.Title != null ? entity.Title[language].ToString() : "(no title)",
                        ModifiedTimeUtc = (entity.Modified == DateTime.MinValue ? DateTime.Now.Date.AddHours(DateTime.Now.Hour) : entity.Modified).ToUniversalTime(),
                        UniqueKey = "2sxc-" + moduleInfo.ModuleID + "-" + (entity.EntityGuid != new Guid() ? entity.EntityGuid.ToString() : (stream.Key + "-" + entity.EntityId)),
                        IsActive = true,
                        TabId = moduleInfo.TabID,
                        PortalId = moduleInfo.PortalID
                    };

                    // Take the newest value (from ContentGroupItem and Entity)
                    if (entity is IHasEditingData)
                    {
                        var contentGroupItemModifiedUtc = ((IHasEditingData) entity).ContentGroupItemModified.ToUniversalTime();
                        searchInfo.ModifiedTimeUtc = searchInfo.ModifiedTimeUtc > contentGroupItemModifiedUtc
                            ? searchInfo.ModifiedTimeUtc
                            : contentGroupItemModifiedUtc;
                    }

                    return searchInfo;
                }));
            }

            try
            {
                engine.CustomizeSearch(searchInfoDictionary, moduleInfo, beginDate);
            }
            catch (Exception e)
            {
                Exceptions.LogException(new SearchIndexException(moduleInfo, e));
            }

            foreach (var searchInfoList in searchInfoDictionary)
            {
                // Filter by Date - take only SearchDocuments that changed since beginDate
                var searchDocumentsToAdd = searchInfoList.Value.Where(p => p.ModifiedTimeUtc >= beginDate.ToUniversalTime()).Select(p => (SearchDocument) p);

                searchDocuments.AddRange(searchDocumentsToAdd);
            }

            return searchDocuments;
        }

        private string StripHtmlAndHtmlDecode(string Text)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(Text, "<.*?>", string.Empty));
        }

        /// <summary>
        /// Gets a string that represents all entities joined with a comma , separator
        /// Does just include String and Number fields
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private string GetJoinedAttributes(IEntity entity, string language)
        {
            return String.Join(", ",
                entity.Attributes.Where(x => x.Value.Type == "String" || x.Value.Type == "Number").Select(x => x.Value[language])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}