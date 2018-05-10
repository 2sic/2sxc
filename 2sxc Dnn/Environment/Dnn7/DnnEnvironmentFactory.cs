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
        public PermissionCheckBase ItemPermissions(IAppIdentity appIdentity, IEntity targetItem, Log parentLog, IInstanceInfo module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, appIdentity: appIdentity);

        public PermissionCheckBase TypePermissions(IAppIdentity appIdentity, IContentType targetType, IEntity targetItem, Log parentLog, IInstanceInfo module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, appIdentity: appIdentity);

        public PermissionCheckBase InstancePermissions(Log parentLog, IInstanceInfo module, IApp app)
            => new DnnPermissionCheck(parentLog, instance: module, app: app);

        public IPagePublishing PagePublisher(Log parentLog) => new PagePublishing(parentLog);

        public IEnvironment Environment(Log parentLog) => new DnnEnvironment(parentLog);




        public AppAndDataHelpersBase AppAndDataHelpers(SxcInstance sxc) => new DnnAppAndDataHelpers(sxc);
    }
}