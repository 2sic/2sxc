using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Helper to work with secure / encrypted data. 
    /// </summary>
    public interface ISecureDataService
    {
        ISecureData<string> Parse(string value);
    }
}
