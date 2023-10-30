using System.Linq;
using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.LookUp;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi.Admin.Query
{
    public class QueryControllerReal: QueryControllerBase<QueryControllerReal>
    {
        private readonly AppWorkSxc _appWorkSxc;
        public const string LogSuffix = "Query";

        private readonly LazySvc<CmsManager> _cmsManagerLazy;
        private readonly IAppStates _appStates;
        private readonly IContextResolver _contextResolver;
        private readonly AppConfigDelegate _appConfigMaker;

        public QueryControllerReal(
            MyServices services,
            AppWorkSxc appWorkSxc,
            LazySvc<CmsManager> cmsManagerLazy,
            IAppStates appStates,
            IContextResolver contextResolver,
            AppConfigDelegate appConfigMaker
        ) : base(services, "Api." + LogSuffix)
        {
            ConnectServices(
                _appWorkSxc = appWorkSxc,
                _cmsManagerLazy = cmsManagerLazy,
                _appStates = appStates,
                _contextResolver = contextResolver,
                _appConfigMaker = appConfigMaker
            );
        }
        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations.
        /// Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        public bool DeleteIfUnused(int appId, int id)
        {
            var l = Log.Fn<bool>($"{nameof(appId)}: {appId}; {nameof(id)}: {id}");

            // Stop if views still use this Query
            var viewUsingQuery = _appWorkSxc.AppViews(appId: appId).GetAll()
                .Where(t => t.Query?.Id == id)
                .Select(t => t.Id)
                .ToArray();

            if (viewUsingQuery.Any())
                throw l.Done(new Exception($"Query is used by Views and cant be deleted. Query ID: {id}. TemplateIds: {string.Join(", ", viewUsingQuery)}"));

            var cms = _cmsManagerLazy.Value.InitQ(_appStates.IdentityOfApp(appId));
            return l.Return(cms.Queries.Delete(id));
        }
        


        public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25) 
            => DebugStream(appId, id, top, LookUpEngineWithBlockRequired(), from, @out);

        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        public QueryRunDto RunDev(int appId, int id, int top)
            => RunDevInternal(appId, id, LookUpEngineWithBlockRequired(), top, builtQuery => builtQuery.Main);

        private LookUpEngine LookUpEngineWithBlockRequired()
        {
            var block = _contextResolver.BlockRequired();
            var lookUps = _appConfigMaker
                .GetLookupEngineForContext(block.Context, block.App, block);
            return lookUps;
        }

    }
}
