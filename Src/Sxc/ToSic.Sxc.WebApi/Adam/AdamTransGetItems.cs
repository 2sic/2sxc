using System;

namespace ToSic.Sxc.WebApi.Adam
{
    /// <summary>
    /// Backend for the API
    /// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
    /// </summary>
    public class AdamTransGetItems<TFolderId, TFileId> : AdamTransactionBase<AdamTransGetItems<TFolderId, TFileId>, TFolderId, TFileId>
    {
        public AdamTransGetItems(Lazy<AdamState<TFolderId, TFileId>> adamState) : base(adamState, "Adm.TrnItm") { }
    }
}
