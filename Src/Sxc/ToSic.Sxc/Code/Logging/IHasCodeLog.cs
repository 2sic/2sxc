namespace ToSic.Sxc.Code;

/// <summary>
/// Marks all Razor / WebAPI classes which provide logging functionality
/// </summary>
[PrivateApi("Was InternalAPI till v17 - This is internal for documentation only, you should never access this interface")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IHasCodeLog
{
    /// <summary>
    /// The logger for the current Razor / WebApi which allows you to add logs to Insights.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    ICodeLog Log { get; }
}