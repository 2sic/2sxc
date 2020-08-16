using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Blocks
{
    internal sealed class BlockFromEntity: BlockBase
    {
        internal const string CbPropertyApp = "App";
        internal const string CbPropertyTitle = "Title";
        internal const string CbPropertyContentGroup = "ContentGroup";
        // 2020-08-14 #2146 2dm believe unused
        ///// <summary>
        ///// Used to store template picker state - so reloads still show the template picker
        ///// TODO: not sure if it's still in use, I don't see it really active in the inpage.js
        ///// </summary>
        //internal const string CbPropertyShowChooser = "ShowTemplateChooser";
        
        // 2020-08-16 clean-up #2148
        //public override bool ParentIsEntity => false;

        #region ContentBlock Definition Entity

        internal IEntity ContentBlockEntity;
        private string _appName;

        private IBlockIdentifier ParseContentBlockDefinition(IEntity cbDefinition)
        {
            ContentBlockEntity = cbDefinition;
            _appName = ContentBlockEntity.GetBestValue(CbPropertyApp)?.ToString() ?? "";

            string temp = ContentBlockEntity.GetBestValue(CbPropertyContentGroup)?.ToString() ?? "";
            Guid.TryParse(temp, out var contentGroupGuid);

            temp = ContentBlockEntity.GetBestValue(ViewParts.TemplateContentType)?.ToString() ?? "";
            Guid.TryParse(temp, out var previewTemplateGuid);

            // 2020-08-14 #2146 2dm believe unused
            //temp = ContentBlockEntity.GetBestValue(CbPropertyShowChooser)?.ToString() ?? "";
            //if (bool.TryParse(temp, out var show))
            //    ShowTemplateChooser = show;

            var appId = new ZoneRuntime(ZoneId, Log).FindAppId(_appName);
            return new BlockIdentifier(ZoneId, appId, contentGroupGuid, previewTemplateGuid);
        }
        #endregion

        #region Constructor and DI

        public BlockFromEntity() : base("CB.Ent") { }

        public BlockFromEntity Init(IBlock parent, IEntity cbDefinition, ILog parentLog = null)
        {
            Init(parent.App.Tenant, parent, parentLog);
            return _constructor(parent, cbDefinition);
        }

        #endregion


        public BlockFromEntity Init(IBlock parent, int contentBlockId, ILog parentLog)
        {
            Init(parent.App.Tenant, parent, parentLog);
            var wrapLog = Log.Call();
            contentBlockId = Math.Abs(contentBlockId); // for various reasons this can be introduced as a negative value, make sure we neutralize that
            var cbDef = parent.BlockBuilder.App.Data.List.One(contentBlockId);  // get the content-block definition
            _constructor(parent, cbDef);
            wrapLog($"ok, id:{contentBlockId}");
            return this;
        }

        private BlockFromEntity _constructor(IBlock parent, IEntity cbDefinition)
        {
            var wrapLog = Log.Call<BlockFromEntity>();
            Parent = parent;
            var blockId = ParseContentBlockDefinition(cbDefinition);
            //ParentId = parent.ParentId;
            //ContentBlockId = -cbDefinition.EntityId; 

            // Ensure we know what portal the stuff is coming from
            //Tenant = Parent.App.Tenant;

            //ZoneId = Parent.ZoneId;

            // Must override previous AppId, as that was of the container-block
            // but the current instance can be of another block
            AppId = blockId.AppId; // new ZoneRuntime(ZoneId, Log).FindAppId(_appName);
            IsContentApp = _appName == Eav.Constants.DefaultAppName;

            //if (AppId == AppConstants.AppIdNotFound)
            //{
            //    _dataIsMissing = true;
            //    wrapLog("data is missing, will stop here");
            //    return this;
            //}

            // 2018-09-22 new, must come before the AppId == 0 check
            //BlockBuilder = new BlockBuilder(pBlock, this, pBlock.Container, Log);

            //if (AppId == 0)
            //{
            //    wrapLog("ok");
            //    return this;
            //}

            //App = new App(pBlock.Environment, Tenant).Init(this, ConfigurationProvider.Build(BlockBuilder, false), true, Log);

            // 2019-11-11 2dm new, with CmsRuntime
            //var cms = new CmsRuntime(App, Log, pBlock.UserMayEdit, pBlock.Environment.PagePublishing.IsEnabled(pBlock.Container.Id));

            //Configuration = cms.Blocks.GetOrGeneratePreviewConfig(_contentGroupGuid, _previewTemplateGuid);

            //// handle cases where the content group is missing - usually because of incomplete import
            //if (Configuration.DataIsMissing)
            //{
            //    _dataIsMissing = true;
            //    App = null;
            //    return this;
            //}

            //// use the content-group template, which already covers stored data + module-level stored settings
            //((BlockBuilder)BlockBuilder).SetTemplateOrOverrideFromUrl(Configuration.View);
            CompleteInit<BlockFromEntity>(parent.BlockBuilder, parent.BlockBuilder.Container, blockId, -cbDefinition.EntityId);
            return wrapLog("ok", this);
        }



        //public override bool IsContentApp => _appName == Eav.Constants.DefaultAppName;

    }
}