using System;
using Oqtane.Repository;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    /// <summary>
    /// WIP - separating concerns in OqtState to get the block and provide the state...
    /// </summary>
    public class OqtGetBlock: HasLog<OqtGetBlock>
    {
        public OqtGetBlock(
            IServiceProvider serviceProvider,
            Lazy<IModuleRepository> modRepoLazy,
            RequestHelper requestHelper,
            IContextResolver contextResolverToInit
        ) : base($"{OqtConstants.OqtLogPrefix}.GetBlk")
        {
            _serviceProvider = serviceProvider;
            _modRepoLazy = modRepoLazy;
            this.requestHelper = requestHelper;
            _contextResolverToInit = contextResolverToInit;
        }
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<IModuleRepository> _modRepoLazy;
        private readonly RequestHelper requestHelper;
        private readonly IContextResolver _contextResolverToInit;

        public IContextResolver TryToLoadBlockAndAttachToResolver()
        {
            if (alreadyTriedToLoad) return _contextResolverToInit;
            alreadyTriedToLoad = true;

            var block = GetBlock();
            _contextResolverToInit.AttachRealBlock(() => block);
            return _contextResolverToInit;
        }

        private bool alreadyTriedToLoad;


        private IBlock GetBlock()
        {
            if (_block != null || _triedToGetBlock) return _block;
            _block = InitializeBlock();
            _triedToGetBlock = true;
            return _block;
        }
        private IBlock _block;
        private bool _triedToGetBlock;


        internal IBlock InitializeBlock()
        {
            var wrapLog = Log.Call<IBlock>();

            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            var moduleId = requestHelper.GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderInstanceId, -1);
            var pageId = requestHelper.GetTypedHeader(ContextConstants.PageIdKey, -1);

            if (moduleId == -1 || pageId == -1)
            {
                moduleId = requestHelper.GetQueryString(WebApiConstants.ModuleId, requestHelper.GetRouteValuesString(WebApiConstants.ModuleId, -1));
                pageId = requestHelper.GetQueryString(WebApiConstants.PageId, requestHelper.GetRouteValuesString(WebApiConstants.PageId, -1));

                if (moduleId == -1 || pageId == -1)
                    return wrapLog("not found", null);

                Log.Add($"Found page/module {pageId}/{moduleId} in route");
            }

            var module = _modRepoLazy.Value.GetModule(moduleId);
            var ctx = _serviceProvider.Build<IContextOfBlock>().Init(pageId, module, Log);

            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            ctx.Page.ParametersInternalOld = OriginalParameters.GetOverrideParams(ctx.Page.ParametersInternalOld);
            var block = _serviceProvider.Build<BlockFromModule>().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            var contentBlockId = requestHelper.GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            if (contentBlockId >= 0) return wrapLog("found block", block);

            Log.Add($"Inner Content: {contentBlockId}");
            var entityBlock = _serviceProvider.Build<BlockFromEntity>().Init(block, contentBlockId, Log);
            return wrapLog("found inner block", entityBlock);
        }
    }
}
