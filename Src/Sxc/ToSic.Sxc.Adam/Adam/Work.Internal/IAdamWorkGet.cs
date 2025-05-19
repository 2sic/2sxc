using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

public interface IAdamWorkGet: IAdamWork//, IHasOptions<AdamWorkOptions>
{
    AdamFolderFileSet ItemsInField(string subFolderName, bool autoCreate = false);
}