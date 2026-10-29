using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// Current-Context Extensions, only used in WebAPIs
/// </summary>
[PrivateApi]
public static class ISxcCurrentContextServiceExtensions
{
    extension(IContextCompleteService ctx)
    {
        public IBlock BlockRequired()
            => ctx.BlockOrNull
               ?? throw new("Block required but missing. It was not attached");

        /// <summary>
        /// Return the block or throw an error
        /// </summary>
        public IContextOfBlock BlockContextRequired()
            => ctx.BlockOrNull?.Context
               ?? throw new("Block context required but not known. It was not attached.");

        /// <summary>
        /// Return the block if known, or null if not
        /// </summary>
        /// <returns>The current block or null</returns>
        public IContextOfBlock? BlockContextOrNull()
            => ctx.BlockOrNull?.Context;
    }
}