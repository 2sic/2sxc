using System;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi
{
    public class QueryBackend: QueryApi
    {

        public QueryBackend(Dependencies dependencies, Lazy<CmsManager> cmsManagerLazy, IAppStates appStates) : base(dependencies)
        {
            _cmsManagerLazy = cmsManagerLazy;
            _appStates = appStates;
        }
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IAppStates _appStates;

        public bool DeleteIfUnused(int appId, int id)
            => _cmsManagerLazy.Value
                .Init(_appStates.Identity(null, appId), true, Log)
                .DeleteQueryIfNotUsedByView(id, Log);

    }
}
