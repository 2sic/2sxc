using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Carries information about what compatibility level to use. Important for components that have an older and newer API.
    /// </summary>
    [PrivateApi("this is just fyi, was published as internal till v14")]
    public interface ICompatibilityLevel
    {
        /// <summary>
        /// The compatibility level to use. 
        /// </summary>
        int CompatibilityLevel { get; }
    }
}
