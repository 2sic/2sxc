using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Web.UI;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Engines;

namespace ToSic.SexyContent.Search
{
    public class SearchController
    {

        public IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();

            var isContentModule = moduleInfo.DesktopModule.ModuleName == "2sxc";

            // New Context because PortalSettings.Current is null
            var zoneId = SexyContent.GetZoneID(moduleInfo.OwnerPortalID);

            if (!zoneId.HasValue)
                return searchDocuments;

            var appId = SexyContent.GetDefaultAppId(zoneId.Value);

            if (!isContentModule)
            {
                // Get AppId from ModuleSettings
                var appIdString = moduleInfo.ModuleSettings[SexyContent.AppIDString];
                if (appIdString == null || !int.TryParse(appIdString.ToString(), out appId))
                    return searchDocuments;
            }

            var sexy = new SexyContent(zoneId.Value, appId, true, moduleInfo.OwnerPortalID);
            var language = moduleInfo.CultureCode;

            // This list will hold all EAV entities to be indexed
            var dataSource = sexy.GetViewDataSource(moduleInfo.ModuleID, false, true);
            var moduleDataSource = (ModuleDataSource)((IDataTarget)dataSource).In["Default"].Source;

            var elements = moduleDataSource.ContentElements.ToList();
            var listElement = moduleDataSource.ListElement;
            if(listElement != null)
                elements.Add(listElement);

            if (!elements.Any() || !elements.Any(e => e.TemplateId.HasValue))
                return searchDocuments;

            var template = sexy.TemplateContext.GetTemplate(elements.First().TemplateId.Value);
            var engine = EngineFactory.CreateEngine(template);
            engine.Init(template, sexy.App, moduleInfo, dataSource, InstancePurposes.IndexingForSearch, sexy);

            try
            {
                engine.CustomizeData();
            }
            catch (Exception e) // Catch errors here, because of references to Request etc.
            {
                Exceptions.LogException(e);
            }

            
            var searchInfoDictionary = new Dictionary<string, List<ISearchInfo>>();

            // Get DNN SearchDocuments from 2Sexy SearchInfos
            foreach (var stream in dataSource.Out.Where(p => p.Key != "Presentation" && p.Key != "ListPresentation" ))
            {
                
                var entities = stream.Value.List.Select(p => p.Value);
                var searchInfoList = searchInfoDictionary[stream.Key] = new List<ISearchInfo>();

                searchInfoList.AddRange(entities.Select(entity =>
                {
                    var searchInfo = new SearchInfo()
                    {
                        Entity = entity,
                        Url = "",
                        Description = "",
                        Body = GetJoinedAttributes(entity, language),
                        Title = entity.Title[language].ToString(),
                        ModifiedTimeUtc = entity.Modified.ToUniversalTime(),
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
                Exceptions.LogException(e);
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
                entity.Attributes.Where(x => x.Value.Type == "String" || x.Value.Type == "Number").Select(x => x.Value[new[] {language}])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}