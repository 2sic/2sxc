using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi]
    public sealed class BlockFromModule: BlockBase
    {
        #region Constructor for DI

        /// <summary>
        /// Official constructor, must call Init afterwards
        /// </summary>
        public BlockFromModule(Dependencies dependencies) : base(dependencies, "CB.Mod") { }

        #endregion

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="ctx"></param>
        ///// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule Init(IContextOfBlock ctx)
        {
            Init(ctx, ctx.Module.BlockIdentifier);
            var wrapLog = Log.Fn<BlockFromModule>(timer: true);
            IsContentApp = ctx.Module.IsContent;
            CompleteInit(null, ctx.Module.BlockIdentifier, ctx.Module.Id);
            return wrapLog.ReturnAsOk(this);
        }

    }
}