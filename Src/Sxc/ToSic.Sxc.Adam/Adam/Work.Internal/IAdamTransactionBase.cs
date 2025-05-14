namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAdamTransactionBase
{
    void Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);
}