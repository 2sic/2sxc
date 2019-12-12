using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Dnn;
using IApp = ToSic.Eav.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using PermissionCheckBase = ToSic.Eav.Security.PermissionCheckBase;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnEnvironmentFactory : IEnvironmentFactory, IWebFactoryTemp
    {
        public PermissionCheckBase ItemPermissions(IAppIdentity appIdentity, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, portal: PortalSettings.Current, appIdentity: appIdentity);

        public PermissionCheckBase TypePermissions(IAppIdentity appIdentity, IContentType targetType, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, portal: PortalSettings.Current, appIdentity: appIdentity);

        public PermissionCheckBase InstancePermissions(ILog parentLog, IContainer module, IApp app)
            => new DnnPermissionCheck(parentLog, portal: PortalSettings.Current, instance: module, app: app);

        public IPagePublishing PagePublisher(ILog parentLog) => new PagePublishing(parentLog);

        public IAppEnvironment Environment(ILog parentLog) => new DnnEnvironment(parentLog);




        public Sxc.Web.DynamicCode AppAndDataHelpers(Sxc.Blocks.ICmsBlock cms) => new DynamicCode(cms);
    }
}