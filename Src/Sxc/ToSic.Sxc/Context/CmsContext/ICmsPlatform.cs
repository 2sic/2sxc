using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// General platform information
    /// </summary>
    [WorkInProgressApi("Still WIP")]
    public interface ICmsPlatform
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
