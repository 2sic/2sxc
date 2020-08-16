using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("todo: review how it's used and named, probably doesn't have any DNN stuff in it any more, and then Module is a wrong name")]
    internal sealed class BlockFromModule: BlockBase
    {
        //public IContainer Container;
        // 2020-08-16 clean-up #2148
        //public override bool ParentIsEntity => false;

        #region Constructor for DI

        /// <summary>
        /// Official constructor, must call Init afterwards
        /// </summary>
        public BlockFromModule(): base("CB.Mod") { }

        #endregion

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="container">the dnn module-info</param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        ///// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule Init(ITenant tenant, IContainer container, ILog parentLog/*, IEnumerable<KeyValuePair<string, string>> overrideParams = null*/)
        {
            Init(tenant, container.BlockIdentifier, parentLog);
            var wrapLog = Log.Call<BlockFromModule>();
            //Container = container;
            IsContentApp = container.IsPrimary;
            //var blockId = container.BlockIdentifier;
            //ParentId = container.Id;
            //ContentBlockId = ParentId;

            // Ensure we know what portal the stuff is coming from
            // Important: PortalSettings is null when in search mode
            //Tenant = tenant;

            //ZoneId = container.BlockIdentifier.ZoneId;
            //AppId = container.BlockIdentifier.AppId; 

            //Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            //if (AppId == AppConstants.AppIdNotFound)
            //{
            //    _dataIsMissing = true;
            //    wrapLog("data is missing, will stop here");
            //    return this;
            //}

            // 2018-09-22 new with auto-init-data
            //var urlParams = overrideParams ?? Factory.Resolve<IHttp>().QueryStringKeyValuePairs();
            //BlockBuilder = new BlockBuilder(null, this, Container, Log);

            //if (AppId == 0)
            //{
            //    wrapLog($"ok a:{AppId}, container:{container.Id}, content-group:{Configuration?.Id}");
            //    return this;
            //}

            //Log.Add("real app, will load data");

            //App = new App(BlockBuilder.Environment, Tenant).Init(this,
            //    ConfigurationProvider.Build(BlockBuilder, false), true, Log);

            // 2019-11-11 2dm new, with CmsRuntime
            //var cms = new CmsRuntime(App, Log, BlockBuilder.UserMayEdit,
            //    BlockBuilder.Environment.PagePublishing.IsEnabled(BlockBuilder.Container.Id));

            //Configuration = cms.Blocks.GetOrGeneratePreviewConfig(blockId.Guid, blockId.PreviewView);

            //if (Configuration.DataIsMissing)
            //{
            //    _dataIsMissing = true;
            //    App = null;
            //    return this;
            //}

            //((BlockBuilder) BlockBuilder).SetTemplateOrOverrideFromUrl(Configuration.View);
            CompleteInit<BlockFromModule>(null, container, container.BlockIdentifier, container.Id);
            return wrapLog("ok", this);
        }

        //public override bool IsContentApp => Container.IsPrimary;

    }
}