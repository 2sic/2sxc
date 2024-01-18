namespace ToSic.Sxc.Backend.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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