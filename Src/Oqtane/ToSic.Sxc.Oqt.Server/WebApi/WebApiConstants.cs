namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public static class WebApiConstants
    {
        public const string Auto = "auto";

        // Release routes
        // Names are a bit strange so they are the same length
        // Which helps align values when they are used together later on

        public const string RootNoLanguage = "";
        public const string RootPathOrLang = "{path-or-language}/";
        public const string RootPathNdLang = "{path}/{subpath-or-language}/";

        public const string ApiRootWithNoLang = RootNoLanguage + "api/sxc";
        public const string ApiRootPathOrLang = RootPathOrLang + "api/sxc";
        public const string ApiRootPathNdLang = RootPathNdLang + "api/sxc";
        
        public const string AppRootNoLanguage = RootNoLanguage + "app";
        public const string AppRootPathOrLang = RootPathOrLang + "app";
        public const string AppRootPathNdLang = RootPathNdLang + "app";

        public const string SharedRootNoLanguage = RootNoLanguage + "2sxc/shared";
        public const string SharedRootPathOrLang = RootPathOrLang + "2sxc/shared";
        public const string SharedRootPathNdLang = RootPathNdLang + "2sxc/shared";

        // Beta routes
        public const string WebApiStateRoot = "{alias:int}/api/sxc";

        // QueryStringKeys
        public const string PageId = "pageid";
        public const string ModuleId = "moduleid";
    }
}
