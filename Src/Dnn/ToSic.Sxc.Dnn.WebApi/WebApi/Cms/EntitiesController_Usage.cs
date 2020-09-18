using System;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EntitiesController
    {
        // New feature in 11.03 - Usage Statistics

        public dynamic Usage(int appId, Guid guid) => new EntityBackend().Init(Log).Usage(GetContext(), GetApp(appId), guid);
    }
}
