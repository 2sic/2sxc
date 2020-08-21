using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Save
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    internal abstract class SaveHelperBase: HasLog
    {
        internal IBlockBuilder BlockBuilder;

        protected SaveHelperBase(IBlockBuilder blockBuilder, ILog parentLog, string logName) : base(logName, parentLog)
        {
            BlockBuilder = blockBuilder;
        }

    }
}
