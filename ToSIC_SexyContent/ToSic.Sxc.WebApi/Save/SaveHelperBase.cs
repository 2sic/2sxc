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

        protected SaveHelperBase(IBlock block, ILog parentLog, string logName) 
            : base(logName, parentLog) 
            => Block = block;
    }
}
