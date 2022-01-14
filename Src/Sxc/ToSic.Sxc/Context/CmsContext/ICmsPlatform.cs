using System;
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
        /// 
        /// 🪒 Use in Razor: `CmsContext.Platform.Type`
        /// </summary>
        PlatformType Type { get; }

        /// <summary>
        /// A nice name ID, like "Dnn" or "Oqtane"
        /// 
        /// 🪒 Use in Razor: `CmsContext.Platform.Name`
        /// </summary>
        /// <remarks>
        /// Please be aware that platform names may change with time - like Dnn was once DotNetNuke
        /// So to safely ensure you are detecting the right platform you should focus on the Type attribute. 
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// The platform version
        /// </summary>
        /// <remarks>Added in v13</remarks>
        Version Version { get; }
    }
}
