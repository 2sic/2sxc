using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Run.Context;
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
            Block = ServiceProvider.Build<DnnGetBlock>().GetCmsBlock(Request, true, Log);
        }

        [PrivateApi] public IBlock Block { get; private set; }

        [PrivateApi] protected IBlock GetBlock() => Block;

        #region App-Helpers for anonyous access APIs

        internal AppOfRequest AppFinder => _appOfRequest ?? (_appOfRequest = _build<AppOfRequest>().Init(Log));
        private AppOfRequest _appOfRequest;

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        internal IApp GetApp(int appId) => _build<Apps.App>().Init(ServiceProvider, appId, Log, GetBlock());

        protected IContextOfBlock GetContext()
        {
            // in case the initial request didn't yet find a block builder, we need to create it now
            var context = Block?.Context ?? ServiceProvider.Build<IContextOfBlock>().InitPageOnly();
            return context;
        }

        #endregion
    }
}
