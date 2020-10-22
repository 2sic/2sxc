using ToSic.Eav;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Blocks
{
    public abstract partial class BlockBase
    {
        public bool EditAllowed
        {
            get
            {
                if (_userMayEdit.HasValue) return _userMayEdit.Value;
                return (_userMayEdit = Factory.Resolve<AppPermissionCheck>()
                           .ForAppInInstance(Context, App, Log).UserMay(GrantSets.WriteSomething)).Value;
            }
        }

        private bool? _userMayEdit;
    }
}
