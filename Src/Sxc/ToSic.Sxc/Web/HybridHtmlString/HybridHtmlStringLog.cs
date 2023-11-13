using ToSic.Lib.Logging;
using ToSic.Razor.Markup;


namespace ToSic.Sxc.Web
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal abstract class HybridHtmlStringLog: RawHtmlString, IHasLog
    {
        protected HybridHtmlStringLog(string logName)
        {
            Log = new Log(logName);
        }
        protected HybridHtmlStringLog(ILog parentLog, string logName)
        {
            Log = new Log(logName, parentLog);
        }


        public ILog Log { get; }

    }
}
