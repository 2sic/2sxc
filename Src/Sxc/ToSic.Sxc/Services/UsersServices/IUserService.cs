using ToSic.Eav.Context;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Still WIP")]
    public interface IUserService: INeedsDynamicCodeRoot
    {
        IUser Get(int id);

        IUser Get(string identityToken);
    }
}