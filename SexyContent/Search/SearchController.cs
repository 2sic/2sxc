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
            var zoneId = SexyContent.GetZoneID(moduleInfo.PortalID);

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

            var sexy = new SexyContent(zoneId.Value, appId, true);
            var language = sexy.GetCurrentLanguageName();

            // This list will hold all EAV entities to be indexed
            var dataSource = sexy.GetViewDataSource(moduleInfo.ModuleID, false);
            var moduleDataSource = (ModuleDataSource)((IDataTarget)dataSource).In["Default"].Source;

            var elements = moduleDataSource.ContentElements.ToList();
            elements.Add(moduleDataSource.ListElement);

            if (!elements.Any() || !elements.Any(e => e.TemplateId.HasValue))
                return searchDocuments;

            var template = sexy.TemplateContext.GetTemplate(elements.First().TemplateId.Value);
            var engine = EngineFactory.CreateEngine(template);
            engine.Init(template, sexy.App, moduleInfo, dataSource, InstancePurposes.IndexingForSearch);

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

                searchInfoList.AddRange(entities.Select(entity => new SearchInfo()
                {
                    Entity = entity,
                    Url = "",
                    Description = "",
                    Body = GetJoinedAttributes(entity, language),
                    Title = entity.Title[language].ToString(),
                    // ToDo: Get modified time from entity
                    ModifiedTimeUtc = DateTime.Now.ToUniversalTime(),//((EntityModel)entity).,
                    UniqueKey = entity.EntityGuid.ToString(),
                    IsActive = true,
                    TabId = moduleInfo.TabID,
                    PortalId = moduleInfo.PortalID
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
                searchDocuments.AddRange(searchInfoList.Value.Select(p => (SearchDocument)p));
            }


            // 2SexyContent does not support delta search, so add SearchDocuments for all deleted entries
            //var moduleSearchController = DotNetNuke.Services.Search.Controllers.SearchController.Instance;


            //DotNetNuke.Services.Search.Internals.InternalSearchController.Instance.

            //DotNetNuke.Services.Search.Controllers.SearchController.SetTestableInstance(moduleSearchController);
            //var moduleSearch = moduleSearchController.ModuleSearch(new SearchQuery() { ModuleId = moduleInfo.ModuleID, PortalIds = new[] { moduleInfo.PortalID } });
            //var searchDocumentsToDelete =
            //    moduleSearch.Results.Where(r => searchDocuments.All(s => s.UniqueKey != r.UniqueKey))
            //        .Select(r => new SearchDocument()
            //        {
            //            UniqueKey = r.UniqueKey,
            //            PortalId = r.PortalId,
            //            CultureCode = r.CultureCode,
            //            IsActive = r.IsActive
            //        });

            //searchDocuments.AddRange(searchDocumentsToDelete);


            //var sourceLists = DotNetNuke.Services.Search.Internals.InternalSearchController.Instance.GetSearchContentSourceList(
            //    moduleInfo.PortalID);

            //foreach (var source in sourceLists)
            //{
            //    source.
            //}

            //DotNetNuke.Services.Search.Controllers.SearchController.Instance.


            return searchDocuments;
        }

        private string StripHtmlAndHtmlDecode(string Text)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(Text, "<.*?>", string.Empty));
        }

        private string GetJoinedAttributes(IEntity entity, string language)
        {
            return String.Join(", ",
                entity.Attributes.Select(x => x.Value[new[] {language}])
                    .Where(a => a != null)
                    .Select(a => StripHtmlAndHtmlDecode(a.ToString()))
                    .Where(x => !String.IsNullOrEmpty(x))) + " ";
        }
    }
}