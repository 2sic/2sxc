using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcBlock context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [DnnLogExceptions]
    public class SxcApiControllerBase: DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.CntBas";

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            SharedContextResolver = GetService<IContextResolver>();
            SharedContextResolver.AttachRealBlock(() => BlockOfRequest);
            SharedContextResolver.AttachBlockContext(() => BlockOfRequest?.Context);
        }

        protected IContextResolver SharedContextResolver;

        private IBlock BlockOfRequest
            => _blockOfRequest ?? (_blockOfRequest = GetService<DnnGetBlock>().GetCmsBlock(Request, Log));
        private IBlock _blockOfRequest;

        [PrivateApi] protected IBlock GetBlock() => BlockOfRequest;

    }
}
