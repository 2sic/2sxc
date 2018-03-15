using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnPermissions: IPermissions
    {

        public bool UserMayEditContent(IInstanceInfo instanceInfo) 
            => UserMayEditContent(instanceInfo, null);

        public bool UserMayEditContent(IInstanceInfo instanceInfo, IApp app)
        {
            var moduleInfo = (instanceInfo as InstanceInfo<ModuleInfo>)?.Info;
            if (moduleInfo == null) return false;

            if (DotNetNuke.Security.Permissions.ModulePermissionController
                .HasModuleAccess(DotNetNuke.Security.SecurityAccessLevel.Edit, "EDIT", moduleInfo))
                return true;

            if (app == null)
                return false;

            var perms = new DnnPermissionController(instanceInfo, app.Metadata);
            return perms.UserMay(PermissionGrant.Update);
        }

    }
}