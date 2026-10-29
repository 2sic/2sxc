namespace ToSic.Sxc.Web.Sys.Html;

/// <summary>
/// 2023-11-14 Can't make this internal, would cause trouble, going public is necessary otherwise IResponsiveImage etc. fail
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract record HybridHtmlStringLog: HybridHtmlString, IHasLog
{
    protected HybridHtmlStringLog(string logName)
    {
        Log = LogFactory.Create(logName);
    }

    protected HybridHtmlStringLog(ILog parentLog, string logName)
    {
        Log = LogFactory.Create(logName, parentLog);
    }


    public ILog Log { get; init; }

}
