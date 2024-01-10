using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services;

/// <summary>
/// Helper to work with secure / encrypted data. 
/// </summary>
/// <remarks>
/// History
/// * Added in 2sxc 12.05
/// </remarks>
[PublicApi]
public interface ISecureDataService: IHasLog, ICanDebug
{
    /// <summary>
    /// Read an input value and return a secure data object.
    /// This will contain the readable value and additional information if it was encrypted or not, etc.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ISecureData<string> Parse(string value);

    //[PrivateApi("WIP v15.01")]
    //string Create(string value);
}