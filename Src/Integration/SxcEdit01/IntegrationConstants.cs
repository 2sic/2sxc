using ToSic.Eav.Apps;

namespace IntegrationSamples.SxcEdit01
{
    // #2sxcIntegration

    public class IntegrationConstants
    {
        public const string LogPrefix = "Int";

        // WebAPI Constants
        public const string DefaultRouteRoot = "api/sxc/";

        /// <summary>
        /// In the demo setup this is the blog app on the PC of 2dm
        /// </summary>
        public const int ZoneId = 145;
        public const int AppId = 1369;
        public static IAppIdentity AppIdentity = new AppIdentity(ZoneId, AppId);

        //public const int PageId = 4427;         // probably not needed TODO: VERIFY
        //public const int ModuleId = 9771;       // probably not needed TODO: VERIFY

        // An entity in the test-db - yours will be different
        public const int ItemWithImagesId = 128280;
        public const string ImagesField = "ImageLibrary";

        #region Parameters used in the page to load the scripts and init the $2sxc JS APIs - see _Layout.cshtml

        // URLs of JS to load
        public const string UrlTo2sxcJs = EnvUiRoot + "js/2sxc.api.min.js";
        public const string UrlTo2sxcInPageJs = EnvUiRoot + "dist/inpage/inpage.min.js";

        // Environment settings for the JS to use
        //public const int EnvPageId = 2742;
        public const string EnvApiRoot = "/" + DefaultRouteRoot;    // the api base path
        public const string EnvUiRoot = "/system/sxc/";             // the path above the `js` and `dist` which contains everything
        public const string EnvRvt = "...";                         // This sample doesn't use RequestVerificationTokens

        #endregion

    }
}
