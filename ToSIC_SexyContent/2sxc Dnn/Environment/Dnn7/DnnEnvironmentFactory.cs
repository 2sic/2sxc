using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Blocks;
using IApp = ToSic.Eav.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnEnvironmentFactory : IEnvironmentFactory, IWebFactoryTemp
    {
        public PermissionCheckBase ItemPermissions(IAppIdentity appIdentity, IEntity targetItem, ILog parentLog, IInstanceInfo module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, portal: PortalSettings.Current, appIdentity: appIdentity);

        public PermissionCheckBase TypePermissions(IAppIdentity appIdentity, IContentType targetType, IEntity targetItem, ILog parentLog, IInstanceInfo module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, portal: PortalSettings.Current, appIdentity: appIdentity);

        public PermissionCheckBase InstancePermissions(ILog parentLog, IInstanceInfo module, IApp app)
            => new DnnPermissionCheck(parentLog, portal: PortalSettings.Current, instance: module, app: app);

        public IPagePublishing PagePublisher(ILog parentLog) => new PagePublishing(parentLog);

        public IAppEnvironment Environment(ILog parentLog) => new DnnEnvironment(parentLog);




        public AppAndDataHelpersBase AppAndDataHelpers(/*SxcInstance*/ICmsBlock cms) => new DnnAppAndDataHelpers(cms);
    }
}