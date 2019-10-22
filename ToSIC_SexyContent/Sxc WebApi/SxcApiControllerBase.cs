using System.Web.Http.Controllers;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.AutoDetectContext;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcInstance context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [SxcWebApiExceptionHandling]
    public class SxcApiControllerBase: DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Log.Rename("Api.CntBas");
            SxcInstance = Helpers.GetSxcOfApiRequest(Request, true, Log);
        }

        internal SxcInstance SxcInstance { get; private set; }
        

        internal static DnnAppAndDataHelpers GetContext(SxcInstance sxcInstance, Log log) 
            => new DnnAppAndDataHelpers(sxcInstance, sxcInstance?.Log ?? log);


        #region App-Helpers for anonyous access APIs

        internal AppFinder AppFinder => _appFinder ?? (_appFinder = new AppFinder(PortalSettings, Env.ZoneMapper, ControllerContext, Log));
        private AppFinder _appFinder;

        #endregion
    }
}
