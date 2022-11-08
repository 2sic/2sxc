using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcBlock context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [DnnLogExceptions]
    [PrivateApi("This was only ever used as an internal base class, so it can be modified as needed - just make sure the derived types don't break")]
    [UseOldNewtonsoftForHttpJson]
    public abstract class SxcApiControllerBase<TRealController>: DnnApiControllerWithFixes<TRealController> where TRealController : class, IHasLog<TRealController>
    {
        protected SxcApiControllerBase(string logSuffix) : base(logSuffix) { }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            SharedContextResolver = GetService<IContextResolver>();
            SharedContextResolver.AttachRealBlock(GetBlock);
        }

        protected IContextResolver SharedContextResolver;

        [PrivateApi] protected IBlock GetBlock() => _blockOfRequest.Get(() => GetService<DnnGetBlock>().GetCmsBlock(Request, Log));
        private readonly GetOnce<IBlock> _blockOfRequest = new GetOnce<IBlock>();

    }
}
