using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// General platform information
    /// </summary>
    /// <remarks>
    /// This must be provided through Dependency Injection, Singleton, as it cannot change at runtime.
    /// </remarks>
    [PrivateApi]
    public class Platform
    {
        /// <summary>
        /// The platform type Id from the enumerator - so stored as an int.
        /// </summary>
        public PlatformType Type;

        /// <summary>
        /// A nice name ID, like "Dnn" or "Oqtane"
        /// </summary>
        public string TypeName => Type.ToString();
    }
}
