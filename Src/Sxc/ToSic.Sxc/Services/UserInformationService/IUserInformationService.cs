using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    public interface IUserInformationService: INeedsDynamicCodeRoot
    {
        string PlatformIdentityTokenPrefix();
        UserInformationDto Find(string identityToken);
    }
}