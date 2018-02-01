using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class Permissions: IPermissions
    {
        public ModuleInfo ModuleInfo;

        //public Permissions(ModuleInfo moduleInfo)
        //{
        //    ModuleInfo = moduleInfo;
        //}
        public Permissions(IInstanceInfo instanceInfo)
        {
            ModuleInfo = (instanceInfo as InstanceInfo<ModuleInfo>)?.Info;
        }

        private bool? _userMayEdit;
        public bool UserMayEditContent
        {
            get
            {
                if (_userMayEdit.HasValue)
                    return _userMayEdit.Value;

                _userMayEdit = DotNetNuke.Security.Permissions.ModulePermissionController.HasModuleAccess(DotNetNuke.Security.SecurityAccessLevel.Edit, "EDIT", ModuleInfo);

                return _userMayEdit.Value;
            }
        }

    }
}