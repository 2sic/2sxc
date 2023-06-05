using ToSic.Lib.Logging;
using ToSic.Razor.Markup;


namespace ToSic.Sxc.Web
{
    public abstract class HybridHtmlStringLog: RawHtmlString, IHasLog
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
