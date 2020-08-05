using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Repositories;
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
        /// <inheritdoc />
        public PermissionCheckBase ItemPermissions(IAppIdentity appIdentity, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetItem: targetItem, instance: module, portal: PortalSettings.Current, appIdentity: appIdentity);

        /// <inheritdoc />
        public PermissionCheckBase TypePermissions(IAppIdentity appIdentity, IContentType targetType, IEntity targetItem, ILog parentLog, IContainer module = null) 
            => new DnnPermissionCheck(parentLog, targetType, targetItem, module, portal: PortalSettings.Current, appIdentity: appIdentity);

        /// <inheritdoc />
        public PermissionCheckBase InstancePermissions(ILog parentLog, IContainer module, IApp app)
            => new DnnPermissionCheck(parentLog, portal: PortalSettings.Current, instance: module, app: app);

        /// <inheritdoc />
        public IPagePublishing PagePublisher(ILog parentLog) => new Cms.PagePublishing(parentLog);

        /// <inheritdoc />
        public IAppEnvironment Environment(ILog parentLog) => new DnnEnvironment(parentLog);

        /// <inheritdoc />
        public DynamicCodeRoot AppAndDataHelpers(Blocks.IBlockBuilder blockBuilder) => new DnnDynamicCode(blockBuilder, 9);


        // experimental
        public IAppFileSystemLoader AppFileSystemLoader(int appId, string path, ILog log) => new DnnAppFileSystemLoader(appId, path, PortalSettings.Current, log);

        // experimental
        /// <summary>
        /// This is the simpler signature, which is used from Eav.Core
        /// The more advance signature which can also deliver InputTypes is the AppFileSystemLoader
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public IAppRepositoryLoader AppRepositoryLoader(int appId, string path, ILog log) => AppFileSystemLoader(appId, path, log);
    }
}