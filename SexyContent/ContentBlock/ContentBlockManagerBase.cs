using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{

    internal abstract class ContentBlockManagerBase 
    {
        protected SxcInstance SxcContext;
        protected int ModuleID;

        protected ContentGroup _cg;
        #region methods which the entity-implementation must customize - so it's virtual

        protected virtual ContentGroup ContentGroup
        {
            get { throw new Exception();}
        }

        protected virtual void SavePreviewTemplateId(int templateId)
        {
            throw new Exception("must be implemented first in inherited class");
            // SxcContext.AppContentGroups.SetPreviewTemplateId(ModuleID, templateId);
        }


        internal virtual void SetTemplateChooserState(bool state)
        {
            throw new Exception("must be implemented first in inherited class");
            // DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(ModuleID, Settings.SettingsShowTemplateChooser, state.ToString());
        }

        internal virtual void SetAppId(int? appId)
        {
            throw new Exception("must be implemented first in inherited class");
            // AppHelpers.SetAppIdForModule(SxcContext.ModuleInfo, appId);
        }

        protected virtual IEnumerable<Template> GetSelectableTemplatesForWebApi()
        {
            throw new Exception("must be implemented first in inherited class");
            // return SxcContext.AppTemplates.GetAvailableTemplatesForSelector(ModuleID, SxcContext.AppContentGroups);
        }

        #endregion

        #region methods which are fairly stable / the same across content-block implementations
        public void AddItem(int? sortOrder = null)
        {
            ContentGroup.AddContentAndPresentationEntity("content", sortOrder, null, null);
        }

        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup, bool? newTemplateChooserState = null)
        {
            Guid? result = null;
            var contentGroup = ContentGroup;// SxcContext.AppContentGroups.GetContentGroupForModule(ModuleID);
            if (contentGroup.Exists || forceCreateContentGroup)
                result = SxcContext.AppContentGroups.SaveTemplateId(ModuleID, templateId);
            else
                SavePreviewTemplateId(templateId);

            if (newTemplateChooserState.HasValue)
                SetTemplateChooserState(newTemplateChooserState.Value);

            return result;
        }

        public IEnumerable<object> GetSelectableTemplates()
        {
            return GetSelectableTemplatesForWebApi().Select(t => new { t.TemplateId, t.Name, t.ContentTypeStaticName });
        }


        public IEnumerable<object> GetSelectableApps()
        {
            try
            {
                var zoneId = ZoneHelpers.GetZoneID(SxcContext.ContentBlock.PortalSettings.PortalId); // note: 2016-03-30 2dm changed this, before it was 2sxcContext.PortalId (so th runtime portal)
                return
                    AppManagement.GetApps(zoneId.Value, false, SxcContext.ContentBlock.PortalSettings)
                        .Where(a => !a.Hidden)
                        .Select(a => new { a.Name, a.AppId });
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
                throw e;
            }
        }


        public IEnumerable<object> GetSelectableContentTypes()
        {
            return SxcContext.AppTemplates.GetAvailableContentTypesForVisibleTemplates().Select(p => new { p.StaticName, p.Name });
        }

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

    }
}