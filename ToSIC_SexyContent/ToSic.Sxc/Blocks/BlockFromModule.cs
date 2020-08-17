using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

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
        /// <param name="tenant"></param>
        /// <param name="container">the dnn module-info</param>
        /// <param name="parentLog">a parent-log; can be null but where possible you should wire one up</param>
        ///// <param name="overrideParams">optional override parameters</param>
        public BlockFromModule Init(ITenant tenant, IContainer container, ILog parentLog)
        {
            Init(tenant, container.BlockIdentifier, parentLog);
            var wrapLog = Log.Call<BlockFromModule>();
            IsContentApp = container.IsPrimary;
            CompleteInit<BlockFromModule>(null, container, container.BlockIdentifier, container.Id);
            return wrapLog("ok", this);
        }

    }
}