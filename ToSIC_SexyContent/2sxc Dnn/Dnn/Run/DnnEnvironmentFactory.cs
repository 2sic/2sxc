using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Code;
using IApp = ToSic.Eav.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using PermissionCheckBase = ToSic.Eav.Security.PermissionCheckBase;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironmentFactory : IEnvironmentFactory, IWebFactoryTemp
    {
        public PermissionCheckBase ItemPermissions(IAppIdentity appIdentity, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, portal: PortalSettings.Current, appIdentity: appIdentity);

        public PermissionCheckBase TypePermissions(IAppIdentity appIdentity, IContentType targetType, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, portal: PortalSettings.Current, appIdentity: appIdentity);

        public PermissionCheckBase InstancePermissions(ILog parentLog, IContainer module, IApp app)
            => new DnnPermissionCheck(parentLog, portal: PortalSettings.Current, instance: module, app: app);

        public IPagePublishing PagePublisher(ILog parentLog) => new Sxc.Dnn.Cms.PagePublishing(parentLog);

        public IAppEnvironment Environment(ILog parentLog) => new DnnEnvironment(parentLog);




        public DynamicCodeBase AppAndDataHelpers(Sxc.Blocks.ICmsBlock cms) => new DnnDynamicCode(cms);
    }
}