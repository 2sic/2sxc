using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi
{
    public class QueryBackend: QueryApi
    {

        public QueryBackend(Dependencies dependencies, Lazy<CmsManager> cmsManagerLazy, IAppStates appStates, IContextResolver contextResolver, AppConfigDelegate appConfigMaker) : base(dependencies)
        {
            _cmsManagerLazy = cmsManagerLazy;
            _appStates = appStates;
            _contextResolver = contextResolver;
            _appConfigMaker = appConfigMaker;
        }
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly IAppStates _appStates;
        private readonly IContextResolver _contextResolver;
        private readonly AppConfigDelegate _appConfigMaker;


        public new QueryBackend Init(int appId, ILog parentLog)
        {
            base.Init(appId, parentLog);
            return this;
        }


        public bool DeleteIfUnused(int appId, int id)
            => _cmsManagerLazy.Value
                .Init(_appStates.IdentityOfApp(appId), true, Log)
                .DeleteQueryIfNotUsedByView(id, Log);


        public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25)
        {
            var block = _contextResolver.RealBlockRequired();
            var lookUps = _appConfigMaker.Init(Log)
                .GetConfigProviderForModule(block.Context, block.App, block);
            return DebugStream(appId, id, top, lookUps, @from, @out);
        }


        public QueryRunDto RunDev(int appId, int id, int top)
        {
            var block = _contextResolver.RealBlockRequired();
            var lookUps = _appConfigMaker.Init(Log)
                .GetConfigProviderForModule(block.Context, block.App, block);

            return RunDevInternal(appId, id, lookUps, top, builtQuery => builtQuery.Item1);
        }
    }
}
