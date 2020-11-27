using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    /// <summary>
    /// General platform information
    /// </summary>
    /// <remarks>
    /// This must be provided through Dependency Injection, Singleton, as it cannot change at runtime.
    /// </remarks>
    [WorkInProgressApi("Still WIP")]
    public interface IPlatformLight
    {
        /// <summary>
        /// The platform type Id from the enumerator - so stored as an int.
        /// </summary>
        PlatformType Type { get; }

        /// <summary>
        /// A nice name ID, like "Dnn" or "Oqtane"
        /// </summary>
        string TypeName {get; }
    }
}
