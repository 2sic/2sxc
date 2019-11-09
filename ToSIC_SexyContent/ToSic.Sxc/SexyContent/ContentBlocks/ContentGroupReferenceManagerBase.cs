using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Query;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Internal;
using ToSic.Sxc.Views;

namespace ToSic.SexyContent.ContentBlocks
{

    internal abstract class ContentGroupReferenceManagerBase : HasLog
    {
        protected SxcInstance SxcContext;
        protected int ModuleId;

        private ContentGroup _cGroup;

        internal ContentGroupReferenceManagerBase(SxcInstance sxc): base("CG.RefMan", sxc.Log)
        {
            SxcContext = sxc;
            ModuleId = SxcContext.EnvInstance.Id;
        }


        #region methods which the entity-implementation must customize - so it's virtual

        protected abstract void SavePreviewTemplateId(Guid templateGuid);


        internal abstract void SetAppId(int? appId);

        internal abstract void EnsureLinkToContentGroup(Guid cgGuid);
        internal abstract void UpdateTitle(IEntity titleItem);
        #endregion

        #region methods which are fairly stable / the same across content-block implementations

        protected ContentGroup ContentGroup
            => _cGroup ?? (_cGroup = SxcContext.ContentGroup);

        public void AddItem(int? sortOrder = null)
            => ContentGroup.AddContentAndPresentationEntity(Parts.ContentLower, sortOrder, null, null);
        

        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            Guid? result;
            Log.Add($"save template#{templateId}, CG-exists:{ContentGroup.Exists} forceCreateCG:{forceCreateContentGroup}");

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (ContentGroup.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = ContentGroup.Exists;

                var contentGroupGuid = SxcContext.ContentBlock.App.ContentGroupManager
                    .UpdateOrCreateContentGroup(ContentGroup, templateId);

                if (!existedBeforeSettingTemplate)
                    EnsureLinkToContentGroup(contentGroupGuid);

                result = contentGroupGuid;
            }
            else
            {
                // only set preview / content-group-reference - but must use the guid
                var dataSource = SxcContext.App.Data;
                var templateGuid = dataSource.List.One(templateId).EntityGuid;
                SavePreviewTemplateId(templateGuid);
                result = null; // send null back
            }

            return result;
        }


        public IEnumerable<TemplateUiInfo> GetSelectableTemplates() 
            => SxcContext.App?.ViewManager.GetCompatibleTemplates(SxcContext.App, ContentGroup);        


        public IEnumerable<AppUiInfo> GetSelectableApps()
        {
            Log.Add("get selectable apps");
            var zoneId = SxcContext.Environment.ZoneMapper.GetZoneId(SxcContext.ContentBlock.Tenant.Id);
            return
                AppManagement.GetApps(zoneId, false, SxcContext.ContentBlock.Tenant, Log)
                    .Where(a => !a.Hidden)
                    .Select(a => new AppUiInfo {
                        Name = a.Name,
                        AppId = a.AppId,
                        SupportsAjaxReload = a.Configuration.SupportsAjaxReload ?? false,
                        Thumbnail = a.Thumbnail,
                        Version = a.Configuration.Version ?? ""
                    });
        }

        public IEnumerable<ContentTypeUiInfo> GetSelectableContentTypes()
            => SxcContext.App?.ViewManager.GetContentTypesWithStatus();
        
        public void ChangeOrder([FromUri] int sortOrder, int destinationSortOrder)
        {
            Log.Add($"change order orig:{sortOrder}, dest:{destinationSortOrder}");
            ContentGroup.ReorderEntities(sortOrder, destinationSortOrder);
        }

        public bool Publish(string part, int sortOrder)
        {
            Log.Add($"publish part{part}, order:{sortOrder}");
            var contentGroup = ContentGroup;
            var contEntity = contentGroup[part][sortOrder];
            var presKey = part.ToLower() == Parts.ContentLower ? Parts.PresentationLower : "listpresentation";
            var presEntity = contentGroup[presKey][sortOrder];

            var hasPresentation = presEntity != null;

            var appMan = new AppManager(SxcContext.App.ZoneId, SxcContext.App.AppId);

            // make sure we really have the draft item an not the live one
            var contDraft = contEntity.IsPublished ? contEntity.GetDraft() : contEntity;
            appMan.Entities.Publish(contDraft.RepositoryId);

            if (hasPresentation)
            {
                var presDraft = presEntity.IsPublished ? presEntity.GetDraft() : presEntity;
                appMan.Entities.Publish(presDraft.RepositoryId);
            }

            return true;
        }

        public void RemoveFromList([FromUri] int sortOrder)
        {
            Log.Add($"remove from list order:{sortOrder}");
            var contentGroup = ContentGroup;
            contentGroup.RemoveContentAndPresentationEntities(Parts.ContentLower, sortOrder);
        }

        #endregion

        internal void UpdateTitle()
        {
            Log.Add("update title");
            // check the contentGroup as to what should be the module title, then try to set it
            // technically it could have multiple different groups to save in, 
            // ...but for now we'll just update the current modules title
            // note: it also correctly handles published/unpublished, but I'm not sure why :)

            // re-load the content-group so we have the new title
            var app = SxcContext.App;
            var contentGroup = app.ContentGroupManager.GetContentGroup(ContentGroup.ContentGroupGuid);

            var titleItem = contentGroup.ListContent.FirstOrDefault() ?? contentGroup.Content.FirstOrDefault();

            UpdateTitle(titleItem);
        }


    }
}