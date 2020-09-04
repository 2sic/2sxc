using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("todo: review how it's used and named, probably doesn't have any DNN stuff in it any more, and then Module is a wrong name")]
    internal sealed class BlockFromModule: BlockBase
    {
        #region Constructor for DI

        /// <summary>
        /// Official constructor, must call Init afterwards
        /// </summary>
        public BlockFromModule(): base("CB.Mod") { }

        #endregion

        /// <summary>
        /// Create a module-content block
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        ///// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule Init(IInstanceContext ctx, ILog parentLog)
        {
            Init(ctx, ctx.Container.BlockIdentifier, parentLog);
            var wrapLog = Log.Call<BlockFromModule>();
            IsContentApp = ctx.Container.IsPrimary;
            CompleteInit<BlockFromModule>(null, ctx.Container.BlockIdentifier, ctx.Container.Id);
            return wrapLog("ok", this);
        }

    }
}