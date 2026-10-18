using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys;

[PrivateApi]
public static class ISxcCurrentContextServiceExtensions
{
    extension(ISxcCurrentContextService ctx)
    {
        public IBlock BlockRequired()
            => ctx.BlockOrNull()
               ?? throw new("Block required but missing. It was not attached");

        /// <summary>
        /// Return the block or throw an error
        /// </summary>
        public IContextOfBlock BlockContextRequired()
            => ctx.BlockContextOrNull()
               ?? throw new("Block context required but not known. It was not attached.");

        /// <summary>
        /// Return the block if known, or null if not
        /// </summary>
        /// <returns>The current block or null</returns>
        public IContextOfBlock? BlockContextOrNull()
            => ctx.BlockOrNull()?.Context;
    }
}