using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class Permissions: IPermissions
    {
        public ModuleInfo ModuleInfo;

        public Permissions(ModuleInfo moduleInfo)
        {
            ModuleInfo = moduleInfo;
        }

        private bool? _userMayEdit;
        public bool UserMayEditContent
        {
            get
            {
                if (_userMayEdit.HasValue)
                    return _userMayEdit.Value;

                // 2016-03-30 2rm: changed code below to simpler HadModuleAccess which seems to do the same
                _userMayEdit = DotNetNuke.Security.Permissions.ModulePermissionController.HasModuleAccess(DotNetNuke.Security.SecurityAccessLevel.Edit, "EDIT", ModuleInfo);

                //var okOnModule = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(ModuleInfo);

                //_userMayEdit = okOnModule;
                //// if a user only has tab-edit but not module edit and is not admin, this needs additional confirmation (probably dnn bug)
                //if (!okOnModule)
                //    _userMayEdit = DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT");

                return _userMayEdit.Value;
            }
        }

    }
}