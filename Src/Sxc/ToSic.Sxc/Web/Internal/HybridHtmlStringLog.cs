using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.Internal;

/// <summary>
/// 2023-11-14 Can't make this internal, would cause trouble, going public is necessary otherwise IResponsiveImage etc. fail
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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