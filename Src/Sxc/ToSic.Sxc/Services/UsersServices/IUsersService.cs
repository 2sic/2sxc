using ToSic.Eav.Context;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    public interface IUsersService: INeedsDynamicCodeRoot
    {
        IUser Get(int id);

        IUser Get(string identityToken);
    }
}