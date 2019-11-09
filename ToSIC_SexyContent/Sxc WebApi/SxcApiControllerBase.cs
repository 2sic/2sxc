using System.Web.Http.Controllers;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.AutoDetectContext;
using ToSic.SexyContent.WebApi.Dnn;
using ToSic.Sxc.Blocks;

namespace ToSic.SexyContent.WebApi
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
            CmsBlock = Helpers.GetSxcOfApiRequest(Request, true, Log);
        }

        // todo: probably rename just to sxc
        internal ICmsBlock CmsBlock { get; private set; }


        internal static DnnAppAndDataHelpers GetContext(ICmsBlock cmsInstance, ILog log) 
            => new DnnAppAndDataHelpers(cmsInstance, cmsInstance?.Log ?? log);


        #region App-Helpers for anonyous access APIs

        internal AppFinder AppFinder => _appFinder ?? (_appFinder = new AppFinder(PortalSettings, Env.ZoneMapper, ControllerContext, Log));
        private AppFinder _appFinder;

        #endregion
    }
}
