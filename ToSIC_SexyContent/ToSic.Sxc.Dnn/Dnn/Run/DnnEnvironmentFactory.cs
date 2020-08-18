using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using IApp = ToSic.Eav.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using PermissionCheckBase = ToSic.Eav.Security.PermissionCheckBase;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironmentFactory : IEnvironmentFactory
    {
        /// <inheritdoc />
        public PermissionCheckBase ItemPermissions(IAppIdentity appIdentity, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, portal: PortalSettings.Current, appIdentity: appIdentity);

        /// <inheritdoc />
        public PermissionCheckBase TypePermissions(IAppIdentity appIdentity, IContentType targetType, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, portal: PortalSettings.Current, appIdentity: appIdentity);

        /// <inheritdoc />
        public PermissionCheckBase InstancePermissions(ILog parentLog, IContainer module, IApp app)
            => new DnnPermissionCheck(parentLog, portal: PortalSettings.Current, instance: module, app: app);
    }
}