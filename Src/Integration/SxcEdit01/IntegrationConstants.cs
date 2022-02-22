namespace IntegrationSamples.SxcEdit01
{
    // #2sxcIntegration

    public class IntegrationConstants
    {
        public const string LogPrefix = "Int";

        #region Parameters used in the page to load the scripts and init the $2sxc JS APIs - see _Layout.cshtml

        // URLs of JS to load
        public const string UrlTo2sxcJs = "/system/sxc/js/2sxc.api.min.js";
        public const string UrlTo2sxcInPageJs = "/system/sxc/dist/inpage/inpage.min.js";

        // Environment settings for the JS to use
        public const int EnvPageId = 2742;
        public const string EnvApiRoot = "/api/sxc/";       // the api base path
        public const string EnvUiRoot = "/system/sxc/";     // the path above the js/dist which contains everything
        public const string EnvRvt = "...";                 // This sample doesn't use RequestVerificationTokens

        #endregion

    }
}
