namespace ToSic.Sxc.Oqt.Shared
{
    public static class WebApiConstants
    {
        public const string Auto = "auto";

        // Release routes
        // Names are a bit strange so they are the same length
        // Which helps align values when they are used together later on
        public const string ApiRootWithNoLang = "api/sxc";
        public const string ApiRootPathOrLang = "{path-or-language}/api/sxc";
        public const string ApiRootPathNdLang = "{path}/{subpath-or-language}/api/sxc";
        
        public const string AppRootNoLanguage = "app";
        public const string AppRootPathOrLang = "{path-or-language}/app";
        public const string AppRootPathNdLang = "{path}/{subpath-or-language}/app";

        // Beta routes
        public const string WebApiRoot = "api/sxc";
        public const string WebApiStateRoot = "{alias:int}/api/sxc";

        public const string MvcApiLogPrefix = "MAP.";

        // QueryStringKeys
        public const string PageId = "pageid";
        public const string ModuleId = "moduleid";
    }
}
