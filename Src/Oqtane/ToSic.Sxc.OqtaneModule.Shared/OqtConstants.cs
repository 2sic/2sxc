using ToSic.Eav.Documentation;

namespace ToSic.Sxc.OqtaneModule.Shared
{
    /// <summary>
    /// This should only contain constants which should really be final, no WIP or similar
    /// </summary>
    public class OqtConstants
    {
        public const string SiteSettingsName = "Site";
        public const string SiteKeyForZoneId = "EavZoneId";


        public const string UserTokenPrefix = "oqt";

        // #uncertain: maybe should incorporate the virtual path of the application?
        public const string UiRoot = "/Modules/ToSic.Sxc/dist";
        // #uncertain: maybe should be more dynamic
        public const string SiteRoot = "/";



        // not yet sure what this is needed for...
        public const string WwwRoot = "wwwroot/";
    }
}
