using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
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
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        ///// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule Init(IContextOfBlock ctx, ILog parentLog)
        {
            Init(ctx, ctx.Module.BlockIdentifier, parentLog);
            var wrapLog = Log.Fn<BlockFromModule>(startTimer: true);
            IsContentApp = ctx.Module.IsContent;
            CompleteInit(null, ctx.Module.BlockIdentifier, ctx.Module.Id);
            return wrapLog.ReturnAsOk(this);
        }

    }
}