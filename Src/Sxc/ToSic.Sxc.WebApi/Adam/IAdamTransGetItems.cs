using System.Collections.Generic;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Adam
{
    public interface IAdamTransGetItems: IAdamTransactionBase
    {
        /// <summary>
        /// Get a DTO list of items in a field
        /// </summary>
        /// <param name="subFolderName">Optional sub folder, when browsing a sub-folder</param>
        /// <param name="autoCreate">Auto-create the folder requested - default is true</param>
        /// <returns></returns>
        IList<AdamItemDto> ItemsInField(string subFolderName, bool autoCreate = true);
    }
}