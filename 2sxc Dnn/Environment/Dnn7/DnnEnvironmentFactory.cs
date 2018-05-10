using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Interfaces;
using IApp = ToSic.Eav.Apps.Interfaces.IApp;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnEnvironmentFactory : IEnvironmentFactory, IWebFactoryTemp
    {
        public PermissionCheckBase ItemPermissions(IZoneIdentity zone, IEntity targetItem, Log parentLog, IInstanceInfo module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, zone: zone);

        public PermissionCheckBase TypePermissions(IZoneIdentity zone, IContentType targetType, IEntity targetItem, Log parentLog, IInstanceInfo module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, zone: zone);

        public PermissionCheckBase InstancePermissions(Log parentLog, IInstanceInfo module, IApp app)
            => new DnnPermissionCheck(parentLog, instance: module, app: app);

        public IPagePublishing PagePublisher(Log parentLog) => new PagePublishing(parentLog);

        public IEnvironment Environment(Log parentLog) => new DnnEnvironment(parentLog);




        public AppAndDataHelpersBase AppAndDataHelpers(SxcInstance sxc) => new DnnAppAndDataHelpers(sxc);
    }
}