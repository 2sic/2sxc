using DotNetNuke.UI.Modules;
using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class Permissions: IPermissions
    {
        public ModuleInstanceContext ModuleContext;

        public Permissions(ModuleInstanceContext moduleInstanceContext)
        {
            ModuleContext = moduleInstanceContext;
        }

        private bool? _userMayEdit;
        public bool UserMayEditContent
        {
            get
            {
                if (_userMayEdit.HasValue)
                    return _userMayEdit.Value;

                var okOnModule = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(ModuleContext.Configuration);

                _userMayEdit = okOnModule;
                // if a user only has tab-edit but not module edit and is not admin, this needs additional confirmation (probably dnn bug)
                if (!okOnModule)
                    _userMayEdit = DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission("EDIT");

                return _userMayEdit.Value;
            }
        }

    }
}