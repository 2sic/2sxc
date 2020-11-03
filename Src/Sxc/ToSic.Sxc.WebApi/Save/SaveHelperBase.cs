using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Save
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    public abstract class SaveHelperBase<T>: HasLog where T: SaveHelperBase<T>
    {
        internal IBlock Block;

        protected SaveHelperBase(string logName)  : base( logName ) { }

        public T Init(IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            Block = block;
            return this as T;
        }
    }
}
