using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAdamTransactionBase : IAdamWork
{
    //void SetupInternal(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);
}