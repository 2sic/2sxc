using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sys.Services;

namespace ToSic.Sxc.Backend.Adam;

public interface IAdamItemDtoMaker: IServiceWithOptionsToSetup<AdamItemDtoMakerOptions>
{
    IEnumerable<AdamItemDto> Convert(AdamFolderFileSet set);
    AdamItemDto Create(IFile file);
    AdamItemDto Create(IFolder folder);
}