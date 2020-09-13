using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Save
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    internal abstract class SaveHelperBase: HasLog
    {
        internal IBlock Block;

        protected SaveHelperBase(string logName)  : base( logName ) { }

        protected void Init(IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            Block = block;

        }
    }
}
