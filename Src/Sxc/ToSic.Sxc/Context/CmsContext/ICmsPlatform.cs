using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// General platform information
    /// </summary>
    [PublicApi]
    public interface ICmsPlatform
    {
        /// <summary>
        /// The platform type Id from the enumerator - so stored as an int.
        /// </summary>
        PlatformType Type { get; }

        /// <summary>
        /// A nice name ID, like "Dnn" or "Oqtane"
        /// Please be aware that platform names may change with time - like Dnn was once DotNetNuke
        /// So to safely ensure you are detecting the right platform you should focus on the Type attribute. 
        /// </summary>
        string Name {get; }
    }
}
