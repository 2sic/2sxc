namespace ToSic.Sxc.Oqt.Shared
{
    /// <summary>
    /// This should only contain constants which should really be final, no WIP or similar
    /// </summary>
    public class OqtConstants
    {
        public const string SiteSettingsName = "Site";
        public const string SiteKeyForZoneId = "EavZoneId";

        /// <summary>
        /// This must be added to the `HostEnvironment.ContentRootPath` to really get into the content
        /// </summary>
        public const string ContentSubfolder = "Content";

        // todo: must later replace the 1 with {0} for string-replaces to work
        public const string AppAssetsLinkRoot = "1/api/appAssets";

        public const string UserTokenPrefix = "oqt";

        // #uncertain: maybe should incorporate the virtual path of the application?
        public const string UiRoot = "/Modules/ToSic.Sxc";
        // #uncertain: maybe should be more dynamic
        public const string SiteRoot = "/";



        // not yet sure what this is needed for...
        public const string WwwRoot = "wwwroot/";

    }
}
