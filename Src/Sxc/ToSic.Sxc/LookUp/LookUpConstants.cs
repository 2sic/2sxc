using ToSic.Eav.Documentation;

namespace ToSic.Sxc.LookUp
{
    [PrivateApi]
    class LookUpConstants
    {

        /// <summary>
        /// This marks a special LookUp provider which is passed around through the system
        /// As of now, this special source only offers two properties
        /// * a special property which is not a lookup but contains the Instance object which is sometimes needed passed on
        /// * a lookup value ShowDrafts which informs if the user may see drafts or not
        /// </summary>
        // todo: rename sometime to be more generic
        public const string InstanceContext = Eav.LookUp.LookUpConstants.InstanceContext;

        // !Important - these must all always be lower case, as they are often used in comparisons
        public const string SourceModule = "module";
        public const string SourcePage = "page";
        public const string SourceSite = "site";

        public const string KeyId = "id";
        public const string KeyGuid = "guid";

        public const string OldDnnModuleId = "moduleid";
        public const string OldDnnPageSource = "tab";
        public const string OldDnnPageId = "tabid";
        public const string OldDnnSiteSource = "portal";
        public const string OldDnnSiteId = "portalid";

        public const string SourceQuery = "query";
        public const string OldDnnSourceQueryString = "querystring";
    }
}
