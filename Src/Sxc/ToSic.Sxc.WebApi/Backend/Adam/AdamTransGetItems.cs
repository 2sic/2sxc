namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Backend for the API
/// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamTransGetItems<TFolderId, TFileId>(
    AdamTransactionBase<AdamTransGetItems<TFolderId, TFileId>, TFolderId, TFileId>.MyServices services)
    : AdamTransactionBase<AdamTransGetItems<TFolderId, TFileId>, TFolderId, TFileId>(services, "Adm.TrnItm"),
        IAdamTransGetItems;