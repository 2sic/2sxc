using ToSic.Lib.Logging;
using ToSic.Razor.Markup;


namespace ToSic.Sxc.Web
{
    /// <summary>
    /// IMPORTANT: Changed to internal for v16.08. #InternalMaybeSideEffectDynamicRazor
    /// This is how it should be done, but it could have a side-effect in dynamic razor in edge cases where interface-type is "forgotton" by Razor.
    /// Keep unless we run into trouble.
    /// Remove this comment 2024 end of Q1 if all works, otherwise re-document why it must be public
    /// </summary>
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
