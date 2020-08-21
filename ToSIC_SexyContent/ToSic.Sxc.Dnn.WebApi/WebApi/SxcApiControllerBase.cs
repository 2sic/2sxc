using System.Web.Http.Controllers;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
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
            BlockBuilder = Helpers.GetCmsBlock(Request, true, Log);
        }

        [PrivateApi] public IBlockBuilder BlockBuilder { get; private set; }


        #region App-Helpers for anonyous access APIs

        internal AppFinder AppFinder => _appFinder ?? (_appFinder = Factory.Resolve<AppFinder>()
                                            .Init(PortalSettings.PortalId, Env.ZoneMapper, Log));
        private AppFinder _appFinder;

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        internal IApp GetApp(int appId) => AppApiHelpers.GetApp(appId, BlockBuilder, Log);

        #endregion
    }
}
