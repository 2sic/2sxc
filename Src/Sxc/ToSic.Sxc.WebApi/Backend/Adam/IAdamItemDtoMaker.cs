﻿using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Sys.Work;

namespace ToSic.Sxc.Backend.Adam;

public interface IAdamItemDtoMaker: IServiceWithSetup<AdamItemDtoMakerOptions>
{
    IEnumerable<AdamItemDto> Convert(AdamFolderFileSet set);
    AdamItemDto Create(IFile file);
    AdamItemDto Create(IFolder folder);
}