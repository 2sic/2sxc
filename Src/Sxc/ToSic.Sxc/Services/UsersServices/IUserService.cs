using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services;

[PrivateApi("Still WIP")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IUserService: INeedsCodeApiService
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    ICmsUser Get(int id);

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    ICmsUser Get(string identityToken);
}