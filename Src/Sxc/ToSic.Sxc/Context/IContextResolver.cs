using System;
using ToSic.Eav.Context;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// This provides other systems with a context
    /// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
    /// </summary>
    public interface IContextResolver: Eav.Context.IContextResolver
    {
        /// <summary>
        /// Return the block or throw an error
        /// </summary>
        IContextOfBlock BlockContextRequired();

        /// <summary>
        /// Return the block if known, or null if not
        /// </summary>
        /// <returns>The current block or null</returns>
        IContextOfBlock BlockContextOrNull();

        /// <summary>
        /// Return the block if known, or an app context if not
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IContextOfApp GetBlockOrSetApp(int appId);

        IContextOfApp SetAppOrGetBlock(string nameOrPath);

        IContextOfApp SetAppOrNull(string nameOrPath);

        IContextOfApp AppNameRouteBlock(string nameOrPath);

        void AttachBlock(Func<IBlock> getBlock);

        IBlock BlockOrNull();

        IBlock BlockRequired();

    }
}
