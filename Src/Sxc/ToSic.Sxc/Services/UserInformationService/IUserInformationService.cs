using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    public interface IUserInformationService: INeedsDynamicCodeRoot
    {
        string PlatformIdentityTokenPrefix();
        UserInformationDto PlatformUserInformationDto(int userId);
        UserInformationDto Find(string identityToken);
    }
}