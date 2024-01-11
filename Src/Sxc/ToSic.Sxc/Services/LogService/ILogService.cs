namespace ToSic.Sxc.Services;

/// <summary>
/// System-Log Service to add messages to the global log in any platform Dnn/Oqtane
/// </summary>
/// <remarks>
/// As of now this service is still very simple, later we may add methods like `Warn()` or `Error()` but let's wait and see what's needed
/// </remarks>
[PublicApi]
public interface ISystemLogService
{
    /// <summary>
    /// Add a general message to the log.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    void Add(string title, string message);
}