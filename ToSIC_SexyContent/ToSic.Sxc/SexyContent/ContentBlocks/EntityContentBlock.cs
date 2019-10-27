using System;
using ToSic.Eav.Data.Query;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Internal;

namespace ToSic.SexyContent.ContentBlocks
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
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template, App?.ConfigurationProvider /*Configuration*/, Log));

        #region ContentBlock Definition Entity

        internal Eav.Interfaces.IEntity ContentBlockEntity;
        private string _appName;
        private Guid _contentGroupGuid;
        private Guid _previewTemplateGuid;

        private void ParseContentBlockDefinition(Eav.Interfaces.IEntity cbDefinition)
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

        public EntityContentBlock(IContentBlock parent, Eav.Interfaces.IEntity cbDefinition, Log parentLog = null): base(parentLog, "CB.Ent") 
            => _constructor(parent, cbDefinition);

        public EntityContentBlock(IContentBlock parent, int contentBlockId, Log parentLog) : base(parentLog, "CB.Ent")
        {
            contentBlockId = Math.Abs(contentBlockId); // for various reasons this can be introduced as a negative value, make sure we neutralize that
            var cbDef = parent.SxcInstance.App.Data.List.One(contentBlockId);  // get the content-block definition
            _constructor(parent, cbDef);
        }

        private void _constructor(IContentBlock parent, Eav.Interfaces.IEntity cbDefinition)
        {
            Parent = parent;
            ParseContentBlockDefinition(cbDefinition);
            ParentId = parent.ParentId;
            ContentBlockId = -cbDefinition.EntityId; 

            // Ensure we know what portal the stuff is coming from
            Tenant = Parent.App.Tenant;

            ZoneId = Parent.ZoneId;

            AppId = AppHelpers.GetAppIdFromGuidName(ZoneId, _appName); // should be 0 if unknown, must test

            if (AppId == Settings.DataIsMissingInDb)
            {
                _dataIsMissing = true;
                return;
            }

            // 2018-09-22 new, must come before the AppId == 0 check
            SxcInstance = new SxcInstance(this, Parent.SxcInstance.EnvInstance, Parent.SxcInstance.Parameters, Log);

            if (AppId == 0) return;

            // 2018-09-22 old
            // try to load the app - if possible
            //App = new App(Tenant, ZoneId, AppId);

            //Configuration = ConfigurationProvider.GetConfigProviderForModule(ParentId, App, SxcInstance);

            // maybe ensure that App.Data is ready
            //var userMayEdit = SxcInstance.UserMayEdit;
            //var publishingEnabled = SxcInstance.Environment.PagePublishing.IsEnabled(Parent.SxcInstance.EnvInstance.Id);
            //App.InitData(userMayEdit, publishingEnabled, Configuration);

            // 2018-09-22 new
            App = new App(Tenant, ZoneId, AppId, ConfigurationProvider.Build(SxcInstance, false), true, Log);

            ContentGroup = App.ContentGroupManager.GetContentGroupOrGeneratePreview(_contentGroupGuid, _previewTemplateGuid);

            // handle cases where the content group is missing - usually because of uncomplete import
            if (ContentGroup.DataIsMissing)
            {
                _dataIsMissing = true;
                App = null;
                return;
            }

            // use the content-group template, which already covers stored data + module-level stored settings
            SxcInstance.SetTemplateOrOverrideFromUrl(ContentGroup.Template);
        }


        //public override SxcInstance SxcInstance
        //    => _sxcInstance ?? (_sxcInstance = new SxcInstance(this, 
        //        Parent.SxcInstance.EnvInstance, 
        //        Parent.SxcInstance.Parameters, 
        //        Log));


        public override bool IsContentApp => _appName == Eav.Constants.DefaultAppName;

    }
}