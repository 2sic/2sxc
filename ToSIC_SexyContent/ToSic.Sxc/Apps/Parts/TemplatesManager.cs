using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    // todo: move "back" into Sxc

    /// <inheritdoc />
    /// <summary>
    /// Templates manager for the app engine - in charge of importing / modifying templates at app-level
    /// </summary>
    public class TemplatesManager: ManagerBase
    {
        public TemplatesManager(AppManager app, ILog parentLog) : base(app, parentLog, "App.TplMng") {}

        #region Template

        /// <summary>
        /// Adds or updates a template - will create a new template if templateId is not specified
        /// </summary>
        public void CreateOrUpdate(int? templateId, string name, string path, string contentTypeStaticName,
            int? contentDemoEntity, string presentationTypeStaticName, int? presentationDemoEntity,
            string listContentTypeStaticName, int? listContentDemoEntity, string listPresentationTypeStaticName,
            int? listPresentationDemoEntity, string templateType, bool isHidden, string location, bool useForList,
            bool publishData, string streamsToPublish, int? queryEntity, string viewNameInUrl)
        {
            var values = new Dictionary<string, object>
            {
                { "Name", name },
                { "Path", path },
                { "ContentTypeStaticName", contentTypeStaticName },
                { "ContentDemoEntity", contentDemoEntity.HasValue ? new List<int> { contentDemoEntity.Value } : new List<int>() },
                { "PresentationTypeStaticName", presentationTypeStaticName },
                { "PresentationDemoEntity", presentationDemoEntity.HasValue ? new List<int> { presentationDemoEntity.Value } : new List<int>() },
                { "ListContentTypeStaticName", listContentTypeStaticName },
                { "ListContentDemoEntity", listContentDemoEntity.HasValue ? new List<int> { listContentDemoEntity.Value } : new List<int>() },
                { "ListPresentationTypeStaticName", listPresentationTypeStaticName },
                { "ListPresentationDemoEntity", listPresentationDemoEntity.HasValue ? new List<int> { listPresentationDemoEntity.Value } : new List<int>() },
                { "Type", templateType },
                { "IsHidden", isHidden },
                { "Location", location },
                { "UseForList", useForList },
                { "PublishData", publishData },
                { "StreamsToPublish", streamsToPublish },
                { "Pipeline", queryEntity.HasValue ? new List<int> { queryEntity.Value } : new List<int>() },
                { "ViewNameInUrl", viewNameInUrl }
            };

            if (templateId.HasValue)
                AppManager.Entities.UpdateParts(templateId.Value, values);
            else
                AppManager.Entities.Create(Configuration.TemplateContentType, values);
        }



        #endregion

    }
}
