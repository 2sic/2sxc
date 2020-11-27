using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi]
    public sealed class BlockFromModule: BlockBase
    {
        #region Constructor for DI

        /// <summary>
        /// Official constructor, must call Init afterwards
        /// </summary>
        public BlockFromModule(Lazy<BlockDataSourceFactory> bdsFactoryLazy) : base(bdsFactoryLazy, "CB.Mod") { }

        #endregion

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        ///// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule Init(IContextOfBlock ctx, ILog parentLog)
        {
            Init(ctx, ctx.Container.BlockIdentifier, parentLog);
            var wrapLog = Log.Call<BlockFromModule>();
            IsContentApp = ctx.Container.IsPrimary;
            CompleteInit<BlockFromModule>(null, ctx.Container.BlockIdentifier, ctx.Container.Id);
            return wrapLog("ok", this);
        }

    }
}