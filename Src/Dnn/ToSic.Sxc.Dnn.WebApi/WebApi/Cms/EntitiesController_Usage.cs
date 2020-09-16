using System;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EntitiesController
    {
        #region New feature in 11.03 - Usage Statitics

        public dynamic Usage(int appId, Guid guid) => new EntityBackend().Init(Log).Usage(GetContext(), GetApp(appId), guid);

        #endregion
    }
}
