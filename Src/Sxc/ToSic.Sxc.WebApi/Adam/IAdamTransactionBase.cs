using System;

namespace ToSic.Sxc.WebApi.Adam
{
    public interface IAdamTransactionBase
    {
        void Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);
    }
}