using System;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// This provides other systems with a context
    /// Note that it's important to always make this **Scoped**, not transient, as there is some re-use after initialization
    /// </summary>
    public interface IContextResolver: IHasLog<IContextResolver>
    {
        /// <summary>
        /// This is the most basic kind of context. ATM you could also inject it directly,
        /// but we want to introduce the capability of giving a static site or something
        /// without having to write code implementing IContextOfSite
        /// </summary>
        /// <returns></returns>
        IContextOfSite Site();

        IContextOfApp App(int appId);

        /// <summary>
        /// Return the block or throw an error
        /// </summary>
        IContextOfBlock BlockRequired();

        /// <summary>
        /// Return the block if known, or null if not
        /// </summary>
        /// <returns>The current block or null</returns>
        IContextOfBlock BlockOrNull();

        /// <summary>
        /// Return the block if known, or an app context if not
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IContextOfApp BlockOrApp(int appId);

        IContextOfApp AppOrBlock(string nameOrPath);

        IContextOfApp AppOrNull(string nameOrPath);

        IContextOfApp AppNameRouteBlock(string nameOrPath);

        void AttachRealBlock(Func<IBlock> getBlock);

        IBlock RealBlockOrNull();

        IBlock RealBlockRequired();

    }
}
