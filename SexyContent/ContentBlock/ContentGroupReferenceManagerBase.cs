using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{

    internal abstract class ContentGroupReferenceManagerBase 
    {
        protected SxcInstance SxcContext;
        protected int ModuleId;

        protected ContentGroup CGroup;

        internal ContentGroupReferenceManagerBase(SxcInstance sxc)
        {
            SxcContext = sxc;
            ModuleId = SxcContext.ModuleInfo.ModuleID;
        }

        

        #region methods which the entity-implementation must customize - so it's virtual

        protected abstract void SavePreviewTemplateId(Guid templateGuid, bool? newTemplateChooserState = null);

        internal abstract void SetTemplateChooserState(bool state);

        internal abstract void SetAppId(int? appId);

        internal abstract void EnsureLinkToContentGroup(Guid cgGuid);
        internal abstract void UpdateTitle(IEntity titleItem);
        #endregion

        #region methods which are fairly stable / the same across content-block implementations

        protected ContentGroup ContentGroup
            => CGroup ?? (CGroup = SxcContext.ContentGroup);

        protected IEnumerable<Template> GetSelectableTemplatesForWebApi()
            => SxcContext.App.TemplateManager/*.AppTemplates*/.GetAvailableTemplates(ContentGroup);

        public void AddItem(int? sortOrder = null)
            => ContentGroup.AddContentAndPresentationEntity("content", sortOrder, null, null);
        

        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup, bool? newTemplateChooserState = null)
        {
            Guid? result;
            var contentGroup = ContentGroup;

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (contentGroup.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = ContentGroup.Exists;

                var contentGroupGuid = SxcContext.ContentBlock.App.ContentGroupManager
                    .UpdateOrCreateContentGroup(ContentGroup, templateId);

                if (!existedBeforeSettingTemplate)
                    EnsureLinkToContentGroup(contentGroupGuid);

                result = contentGroupGuid;

                if (newTemplateChooserState.HasValue && newTemplateChooserState.Value != SxcContext.ContentBlock.ShowTemplateChooser)
                    SetTemplateChooserState(newTemplateChooserState.Value);
            }
            else
            {
                // only set preview / content-group-reference - but must use the guid
                var dataSource = SxcContext.App.Data["Default"];
                var templateGuid = dataSource.List[templateId].EntityGuid;
                SavePreviewTemplateId(templateGuid, newTemplateChooserState);
                result = null; // send null back
            }




            return result;
        }


        public IEnumerable<object> GetSelectableTemplates()
        {
            if (SxcContext.App == null) return null; // no app yet, so we also can't give a list of the app
            return GetSelectableTemplatesForWebApi().Select(t => new { t.TemplateId, t.Name, t.ContentTypeStaticName, t.IsHidden });
        }


        public IEnumerable<object> GetSelectableApps()
        {
            try
            {
                var zoneId = ZoneHelpers.GetZoneID(SxcContext.ContentBlock.PortalSettings.PortalId); // note: 2016-03-30 2dm changed this, before it was 2sxcContext.PortalId (so th runtime portal)
                return
                    AppManagement.GetApps(zoneId.Value, false, SxcContext.ContentBlock.PortalSettings)
                        .Where(a => !a.Hidden)
                        .Select(a => new { a.Name, a.AppId, SupportsAjaxReload = a.Configuration.SupportsAjaxReload ?? false });
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
                throw e;
            }
        }


        public IEnumerable<object> GetSelectableContentTypes()
            => SxcContext.App?.TemplateManager./*AppTemplates.*/GetContentTypesWithStatus();
        

        public void ChangeOrder([FromUri] int sortOrder, int destinationSortOrder)
        {
            try
            {
                // var contentGroup = SxcContext.AppContentGroups.GetContentGroupForModule(ModuleID);
                ContentGroup.ReorderEntities(sortOrder, destinationSortOrder);
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
                throw;
            }
        }

        public bool Publish(int repositoryId)
        {
            SxcContext.EavAppContext.Publishing.PublishDraftInDbEntity(repositoryId, true);
            return true;
        }

        public bool Publish(string part, int sortOrder)
        {
            try
            {
                var contentGroup = ContentGroup;// SxcContext.AppContentGroups.GetContentGroupForModule(ModuleID);
                var contEntity = contentGroup[part][sortOrder];
                var presKey = part.ToLower() == "content" ? "presentation" : "listpresentation";
                var presEntity = contentGroup[presKey][sortOrder];

                var hasPresentation = presEntity != null;

                // make sure we really have the draft item an not the live one
                var contDraft = contEntity.IsPublished ? contEntity.GetDraft() : contEntity;
                if (contEntity != null && !contDraft.IsPublished)
                    SxcContext.EavAppContext.Publishing.PublishDraftInDbEntity(contDraft.RepositoryId, !hasPresentation); // don't save yet if has pres...

                if (hasPresentation)
                {
                    var presDraft = presEntity.IsPublished ? presEntity.GetDraft() : presEntity;
                    if (!presDraft.IsPublished)
                        SxcContext.EavAppContext.Publishing.PublishDraftInDbEntity(presDraft.RepositoryId, true);
                }

                return true;
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
                throw;
            }
        }

        public void RemoveFromList([FromUri] int sortOrder)
        {
            try
            {
                var contentGroup = ContentGroup;// SxcContext.AppContentGroups.GetContentGroupForModule(ModuleID);
                contentGroup.RemoveContentAndPresentationEntities("content", sortOrder);
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
                throw;
            }
        }

        #endregion

        internal void UpdateTitle()
        {
            // check the contentGroup as to what should be the module title, then try to set it
            // technically it could have multiple different groups to save in, 
            // ...but for now we'll just update the current modules title
            // note: it also correctly handles published/unpublished, but I'm not sure why :)

            // re-load the content-group so we have the new title
            var app = SxcContext.App;
            var contentGroup = app.ContentGroupManager.GetContentGroup(ContentGroup.ContentGroupGuid);// .GetContentGroupForModule(SxcContext.ModuleInfo.ModuleID);

            IEntity titleItem = contentGroup.ListContent.FirstOrDefault() ?? contentGroup.Content.FirstOrDefault();

            UpdateTitle(titleItem);
            //if (titleItem?.GetBestValue("EntityTitle") != null)
            //    SxcContext.ModuleInfo.ModuleTitle = titleItem.GetBestValue("EntityTitle").ToString();
        }


    }
}