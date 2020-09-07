namespace ToSic.Sxc.WebApi.Adam
{
    /// <summary>
    /// Backend for the API
    /// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
    /// </summary>
    internal class AdamTransGetItems: AdamTransactionBase<AdamTransGetItems>
    {
        public AdamTransGetItems() : base("Adm.BckEnd") { }

    }
}
