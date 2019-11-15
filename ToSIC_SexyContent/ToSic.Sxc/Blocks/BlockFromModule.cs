using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using ToSic.SexyContent;
using ToSic.SexyContent.DataSources;
using ToSic.Sxc.Apps;
using ToSic.Sxc.DnnWebForms.Helpers;
using ToSic.Sxc.Interfaces;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Blocks
{
    internal sealed class BlockFromModule: BlockBase
    {
        public IContainer Container;

        public override BlockEditorBase Editor => new BlockEditorForModule(CmsInstance);

        public override bool ParentIsEntity => false;


        public override IBlockDataSource Data => _dataSource 
            ?? (_dataSource = BlockDataSource.ForContentGroupInSxc(CmsInstance, View, App?.ConfigurationProvider, Log, Container.Id));

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="container">the dnn module-info</param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        /// <param name="tenant"></param>
        /// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule(IContainer container, ILog parentLog, ITenant tenant, IEnumerable<KeyValuePair<string, string>> overrideParams = null): base(parentLog, "CB.Mod")
        {
            var wrapLog = Log.Call("constructor");
            Container = container ?? throw new Exception("Need valid Instance/ModuleInfo / ModuleConfiguration of runtime");
            ParentId = container.Id;
            ContentBlockId = ParentId;

            // Ensure we know what portal the stuff is coming from
            // PortalSettings is null, when in search mode
            Tenant = tenant;

            // important: don't use the SxcInstance.Environment, as it would try to init the Sxc-object before the app is known, causing various side-effects
            var tempEnv = Factory.Resolve<IEnvironmentFactory>().Environment(parentLog);
            ZoneId = tempEnv.ZoneMapper.GetZoneId(tenant.Id); // use tenant as reference, as it can be different from instance.TenantId
            
            AppId = Factory.Resolve<IMapAppToInstance>().GetAppIdFromInstance(container, ZoneId) ?? 0;// fallback/undefined YET

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            if (AppId == Settings.DataIsMissingInDb)
            {
                _dataIsMissing = true;
                Log.Add("data is missing, will stop here");
                return;
            }

            // 2018-09-22 new with auto-init-data
            var urlParams = overrideParams ?? SystemWeb.GetUrlParams();
            CmsInstance = new CmsInstance(this, Container, urlParams, Log);

            if (AppId != 0)
            {
                Log.Add("real app, will load data");

                // 2018-09-22 new with auto-init-data
                App = new App(Tenant, ZoneId, AppId, ConfigurationProvider.Build(CmsInstance, false), true, Log);

                // 2019-11-11 2dm new, with CmsRuntime
                var cms = new CmsRuntime(App, Log, CmsInstance.UserMayEdit,
                    CmsInstance.Environment.PagePublishing.IsEnabled(CmsInstance.Container.Id));

                Configuration = cms.Blocks.GetInstanceContentGroup(container.Id, container.PageId);

                if (Configuration.DataIsMissing)
                {
                    _dataIsMissing = true;
                    App = null;
                    return;
                }

                ((CmsInstance)CmsInstance).SetTemplateOrOverrideFromUrl(Configuration.View);                
            }

            wrapLog($"ok a:{AppId}, container:{container.Id}, content-group:{Configuration?.ContentGroupId}");
        }

        public override bool IsContentApp => Container.IsPrimary;

    }
}