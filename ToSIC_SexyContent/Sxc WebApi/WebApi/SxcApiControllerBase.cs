using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcBlock context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [SxcWebApiExceptionHandling]
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

        internal AppFinder AppFinder => _appFinder ?? (_appFinder = new AppFinder(PortalSettings, Env.ZoneMapper, ControllerContext, Log));
        private AppFinder _appFinder;

        #endregion
    }
}
