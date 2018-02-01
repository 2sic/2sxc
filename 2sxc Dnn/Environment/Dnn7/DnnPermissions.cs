using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnPermissions: IPermissions
    {

        public bool UserMayEditContent(IInstanceInfo instanceInfo)
        {
            var moduleInfo = (instanceInfo as InstanceInfo<ModuleInfo>)?.Info;
            if (moduleInfo == null) return false;

            return DotNetNuke.Security.Permissions.ModulePermissionController
                .HasModuleAccess(DotNetNuke.Security.SecurityAccessLevel.Edit, "EDIT", moduleInfo);
        }

    }
}