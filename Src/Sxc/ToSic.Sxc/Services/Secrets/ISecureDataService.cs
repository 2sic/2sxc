using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    public interface ISecureDataService
    {
        ISecureData<string> Parse(string value);
    }
}
