using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Marks all Razor / WebAPI classes which provide logging functionality
    /// </summary>
    [PrivateApi("This is internal for documentation only, you should never access this interface, was 'Internal' till 16.07")]
    public interface IHasCodeLog
    {
        /// <summary>
        /// The logger for the current Razor / WebApi which allows you to add logs to Insights.
        /// </summary>
        ICodeLog Log { get; }
    }
}
