namespace ToSic.Sxc.Oqt.Shared
{
    public static class WebApiConstants
    {
        public const string Auto = "auto";

        // Release routes
        public const string ApiRoot = "api/sxc";
        public const string ApiRoot2 = "{path-or-language}/api/sxc";
        public const string ApiRoot3 = "{path}/{subpath-or-language}/api/sxc";
        public const string AppRoot = "app";
        public const string AppRoot2 = "{path-or-language}/app";
        public const string AppRoot3 = "{path}/{subpath-or-language}/app";

        // Beta routes
        public const string WebApiRoot = "api/sxc";
        public const string WebApiStateRoot = "{alias:int}/api/sxc";

        public const string MvcApiLogPrefix = "MAP.";

        // QueryStringKeys
        public const string PageId = "pageid";
        public const string ModuleId = "moduleid";
    }
}
