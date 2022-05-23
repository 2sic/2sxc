using ToSic.Eav.Apps.Security;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Blocks
{
    public abstract partial class BlockBase
    {
        //public bool EditAllowed
        //{
        //    get
        //    {
        //        return Context.ShowDrafts;
        //        if (_editAllowed.HasValue) return _editAllowed.Value;
        //        Log.A($"will find if editing is allowed, app is probably null {App}");
        //        return (_editAllowed = Context.ServiceProvider.Build<AppPermissionCheck>()
        //                   .ForAppInInstance(Context, this, Log).UserMay(GrantSets.WriteSomething)).Value;
        //    }
        //}

        //private bool? _editAllowed;
    }
}
