namespace ToSic.Sxc.Backend.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IAdamTransactionBase
{
    void Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);
}