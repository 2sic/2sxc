using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;
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
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Log.Rename("Api.CntBas");
            CmsBlock = Helpers.GetCmsBlock(Request, true, Log);
        }

        [PrivateApi] public ICmsBlock CmsBlock { get; private set; }


        internal static DnnDynamicCode GetContext(ICmsBlock cmsInstance, ILog log) 
            => new DnnDynamicCode(cmsInstance, 10, cmsInstance?.Log ?? log);


        #region App-Helpers for anonyous access APIs

        internal AppFinder AppFinder => _appFinder ?? (_appFinder = new AppFinder(PortalSettings, Env.ZoneMapper, ControllerContext, Log));
        private AppFinder _appFinder;

        #endregion
    }
}
