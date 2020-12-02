using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi.App;
using IApp = ToSic.Sxc.Apps.IApp;

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
            var sharedContextResolver = ServiceProvider.Build<IContextResolver>();
            sharedContextResolver.AttachRealBlock(() => BlockOfRequest);
            sharedContextResolver.AttachBlockContext(() => BlockOfRequest?.Context);
        }


        private IBlock BlockOfRequest => _blockOfRequest ??
                                         (_blockOfRequest = ServiceProvider.Build<DnnGetBlock>().GetCmsBlock(Request, Log));
        private IBlock _blockOfRequest;

        [PrivateApi] protected IBlock GetBlock() => BlockOfRequest;

        /// <summary>
        /// Temporary call to replace GetBlock, so we can gradually filter out the bad uses of Block
        /// </summary>
        /// <returns></returns>
        [PrivateApi] protected IBlock BlockReallyUsedAsBlock() => BlockOfRequest;

        #region App-Helpers for anonyous access APIs

        internal AppOfRequest AppFinder => _appOfRequest ?? (_appOfRequest = _build<AppOfRequest>().Init(Log));
        private AppOfRequest _appOfRequest;

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        internal IApp GetApp(int appId) => _build<Apps.App>().Init(ServiceProvider, appId, Log, GetContext().UserMayEdit);

        //protected IContextOfApp GetAppContext(int appId)
        //{
        //    // First get a normal basic context which is initialized with site, etc.
        //    var appContext = ServiceProvider.Build<IContextOfApp>();
        //    appContext.Init(Log);
        //    appContext.ResetApp(appId);
        //    return appContext;
        //}

        protected IContextOfBlock GetContext()
        {
            if (_context != null) return _context;
            if (BlockOfRequest?.Context != null) return _context = BlockOfRequest.Context;
            // in case the initial request didn't yet find a block builder, we need to create it now
            _context = ServiceProvider.Build<IContextOfBlock>();
            _context.Init(Log);
            _context.InitPageOnly();
            return _context;
        }

        private IContextOfBlock _context;

        #endregion
    }
}
