using System;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal sealed class EntityContentBlock: ContentBlockBase
    {
        internal const string CbPropertyApp = "App";
        internal const string CbPropertyTitle = "Title";
        internal const string CbPropertyContentGroup = "ContentGroup";
        internal const string CbPropertyTemplate = "Template";
        internal const string CbPropertyShowChooser = "ShowTemplateChooser";

        public override ContentGroupReferenceManagerBase Manager => new EntityContentGroupReferenceManager(SxcInstance);
        public override bool ParentIsEntity => false;

        public override ViewDataSource Data => _dataSource 
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template));

        #region ContentBlock Definition Entity

        internal IEntity ContentBlockEntity;
        private string _appName;
        private Guid _contentGroupGuid;
        private Guid _previewTemplateGuid;

        private void ParseContentBlockDefinition(IEntity cbDefinition)
        {
            ContentBlockEntity = cbDefinition;
            _appName = ContentBlockEntity.GetBestValue(CbPropertyApp)?.ToString() ?? "";

            string temp = ContentBlockEntity.GetBestValue(CbPropertyContentGroup)?.ToString() ?? "";
            Guid.TryParse(temp, out _contentGroupGuid);

            temp = ContentBlockEntity.GetBestValue(CbPropertyTemplate)?.ToString() ?? "";
            Guid.TryParse(temp, out _previewTemplateGuid);

            temp = ContentBlockEntity.GetBestValue(CbPropertyShowChooser)?.ToString() ?? "";
            bool show;
            if (bool.TryParse(temp, out show))
                ShowTemplateChooser = show;

        }
        #endregion

        public EntityContentBlock(IContentBlock parent, IEntity cbDefinition)
        {
            _constructor(parent, cbDefinition);
        }

        private void _constructor(IContentBlock parent, IEntity cbDefinition)
        {
            Parent = parent;
            ParseContentBlockDefinition(cbDefinition);
            ParentId = parent.ParentId;
            ContentBlockId = -cbDefinition.EntityId; // "mod:" + ParentId +  "-ent:" + cbDefinition.EntityId;


            // Ensure we know what portal the stuff is coming from
            PortalSettings = Parent.App.OwnerPortalSettings;

            ZoneId = Parent.ZoneId;


            AppId = AppHelpers.GetAppIdFromName(ZoneId, _appName); // should be 0 if unknown, must test

            if (AppId != 0)
            {
                // try to load the app - if possible
                App = new App(PortalSettings, AppId, ZoneId);
                ContentGroup = App.ContentGroupManager.GetContentGroupOrGeneratePreview(_contentGroupGuid, _previewTemplateGuid);

                // use the content-group template, which already covers stored data + module-level stored settings
                Template = ContentGroup.Template;

                // maybe ensure that App.Data is ready
                App.InitData(SxcInstance.Environment.Permissions.UserMayEditContent, Data.ConfigurationProvider);
            }
        }

        public EntityContentBlock(IContentBlock parent, int contentBlockId)
        {
            contentBlockId = Math.Abs(contentBlockId); // for various reasons this can be introduced as a negative value, make sure we neutralize that
            var cbDef = parent.SxcInstance.App.Data["Default"].List[contentBlockId];  // get the content-block definition
            _constructor(parent, cbDef);
        }

        public override SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this, Parent.SxcInstance));


        public override bool IsContentApp => _appName == Constants.DefaultAppName;

    }
}