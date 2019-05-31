using ToSic.SexyContent;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Razor
{
    // temp - should be elsewhere, but quickly need it so Permissions-object still works after refactoring
    public class RazorPermissions
    {
        protected readonly SxcInstance SxcInstance;
        internal RazorPermissions(SxcInstance sxc) => SxcInstance = sxc;

        /// <summary>
        /// This property is used publicly, so it must exist
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool UserMayEditContent => SxcInstance.UserMayEdit;

    }
}
