using ToSic.Lib.Logging;


namespace ToSic.Sxc.Web
{
    public abstract class HybridHtmlStringLog: HybridHtmlString, IHasLog
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
