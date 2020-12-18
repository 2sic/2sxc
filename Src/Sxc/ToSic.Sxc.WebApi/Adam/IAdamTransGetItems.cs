using System.Collections.Generic;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Adam
{
    public interface IAdamTransGetItems: IAdamTransactionBase
    {
        IList<AdamItemDto> ItemsInField(string subFolderName);
    }
}