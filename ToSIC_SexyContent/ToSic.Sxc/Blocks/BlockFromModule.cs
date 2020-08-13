using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Web;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("todo: review how it's used and named, probably doesn't have any DNN stuff in it any more, and then Module is a wrong name")]
    internal sealed class BlockFromModule: BlockBase
    {
        public IContainer Container;

        public override bool ParentIsEntity => false;


        public override IBlockDataSource Data => _dataSource 
            ?? (_dataSource = Block.GetBlockDataSource(BlockBuilder, View, App?.ConfigurationProvider, Log, Container.Id));

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="container">the dnn module-info</param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        /// <param name="tenant"></param>
        /// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule(IContainer container, ILog parentLog, ITenant tenant, IEnumerable<KeyValuePair<string, string>> overrideParams = null): base(parentLog, "CB.Mod")
        {
            var wrapLog = Log.Call();
            Container = container ?? throw new Exception("Need valid Instance/ModuleInfo / ModuleConfiguration of runtime");
            ParentId = container.Id;
            ContentBlockId = ParentId;

            // Ensure we know what portal the stuff is coming from
            // Important: PortalSettings is null when in search mode
            Tenant = tenant;

            ZoneId = container.BlockIdentifier.ZoneId;
            AppId = container.BlockIdentifier.AppId; 

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            if (AppId == AppConstants.AppIdNotFound)
            {
                _dataIsMissing = true;
                Log.Add("data is missing, will stop here");
                return;
            }

            // 2018-09-22 new with auto-init-data
            var urlParams = overrideParams ?? Factory.Resolve<IHttp>().QueryStringKeyValuePairs();
            BlockBuilder = new BlockBuilder(null, this, Container, urlParams, Log);

            if (AppId != 0)
            {
                Log.Add("real app, will load data");

                App = new App(BlockBuilder.Environment, Tenant).Init(this, ConfigurationProvider.Build(BlockBuilder, false), true, Log);

                // 2019-11-11 2dm new, with CmsRuntime
                var cms = new CmsRuntime(App, Log, BlockBuilder.UserMayEdit,
                    BlockBuilder.Environment.PagePublishing.IsEnabled(BlockBuilder.Container.Id));

                Configuration = cms.Blocks.GetInstanceContentGroup(container);

                if (Configuration.DataIsMissing)
                {
                    _dataIsMissing = true;
                    App = null;
                    return;
                }

                ((BlockBuilder)BlockBuilder).SetTemplateOrOverrideFromUrl(Configuration.View);                
            }

            wrapLog($"ok a:{AppId}, container:{container.Id}, content-group:{Configuration?.Id}");
        }

        public override bool IsContentApp => Container.IsPrimary;

    }
}