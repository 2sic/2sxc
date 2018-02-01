using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.Environment.Dnn7.Security
{
    public class DnnEnvironmentFactory : IEnvironmentFactory
    {
        public PermissionController ItemPermissions(IEntity targetItem, Log parentLog, IInstanceInfo module = null) 
            => new DnnPermissionController(targetItem, parentLog, module);

        public PermissionController TypePermissions(IContentType targetType, IEntity targetItem, Log parentLog, IInstanceInfo module = null) 
            => new DnnPermissionController(targetType, targetItem, parentLog, module);

        public IPagePublishing PagePublisher(Log parentLog) => new PagePublishing(parentLog);

        public IEnvironment Environment(Log parentLog) => new DnnEnvironment(parentLog);
    }
}