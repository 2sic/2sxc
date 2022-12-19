using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Helper to work with secure / encrypted data. 
    /// </summary>
    /// <remarks>
    /// History
    /// * Added in 2sxc 12.05
    /// </remarks>
    [PublicApi]
    public interface ISecureDataService: IHasLog
    {
        ISecureData<string> Parse(string value);
    }
}
