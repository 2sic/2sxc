using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

public interface IAdamItemDtoMaker: IHasOptions<AdamItemDtoMakerOptions>
{
    IEnumerable<AdamItemDto> Convert(AdamFolderFileSet set);
    AdamItemDto Create(IFile file);
    AdamItemDto Create(IFolder folder);
}