using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Marks all Razor / WebAPI classes which provide logging functionality
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("This is internal for documentation only, you should never access this interface")]
    public interface IHasCodeLog
    {
        /// <summary>
        /// The logger for the current Razor / WebApi
        /// </summary>
        ICodeLog Log { get; }
    }
}
