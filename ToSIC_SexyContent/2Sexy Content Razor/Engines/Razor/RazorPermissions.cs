using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Engines.Razor
{
    // temp - should be elsewhere, but quickly need it so Permissions-object still works after refactoring
    public class RazorPermissions
    {
        protected readonly /*SxcInstance*/ICmsBlock CmsInstance;
        internal RazorPermissions(/*SxcInstance*/ICmsBlock cms) => CmsInstance = cms;

        /// <summary>
        /// This property is used publicly, so it must exist
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool UserMayEditContent => CmsInstance.UserMayEdit;

    }
}
