using System;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi.Admin.Query
{
    public class QueryControllerReal: QueryControllerBase<QueryControllerReal>
    {
        public const string LogSuffix = "Query";

        public QueryControllerReal(QueryControllerDependencies dependencies, Lazy<CmsManager> cmsManagerLazy, IAppStates appStates, IContextResolver contextResolver, AppConfigDelegate appConfigMaker) 
            : base(dependencies, "Api." + LogSuffix)
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

        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations.
        /// Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        public bool DeleteIfUnused(int appId, int id)
            => _cmsManagerLazy.Value
                .Init(_appStates.IdentityOfApp(appId), true, Log)
                .DeleteQueryIfNotUsedByView(id, Log);


        public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25)
        {
            var block = _contextResolver.RealBlockRequired();
            var lookUps = _appConfigMaker.Init(Log)
                .GetLookupEngineForContext(block.Context, block.App, block);
            return DebugStream(appId, id, top, lookUps, @from, @out);
        }


        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        public QueryRunDto RunDev(int appId, int id, int top)
        {
            var block = _contextResolver.RealBlockRequired();
            var lookUps = _appConfigMaker.Init(Log)
                .GetLookupEngineForContext(block.Context, block.App, block);

            return RunDevInternal(appId, id, lookUps, top, builtQuery => builtQuery.Item1);
        }
    }
}
