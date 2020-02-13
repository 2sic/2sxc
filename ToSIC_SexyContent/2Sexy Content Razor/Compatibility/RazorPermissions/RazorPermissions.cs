using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility.RazorPermissions
{
    /// <summary>
    /// This is a compatibily leftover from old code - new code uses Edit.Enabled
    /// </summary>
    public class RazorPermissions
    {
        protected readonly IBlockBuilder BlockBuilder;
        internal RazorPermissions(IBlockBuilder blockBuilder) => BlockBuilder = blockBuilder;

        /// <summary>
        /// This property is used publicly, so it must exist
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool UserMayEditContent => BlockBuilder.UserMayEdit;

    }
}
