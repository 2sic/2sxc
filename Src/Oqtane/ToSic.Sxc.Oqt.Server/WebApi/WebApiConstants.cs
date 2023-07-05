using System.Collections.Generic;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.EditUi;

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

        // Endpoint mappings
        public static readonly string[] SxcEndpointPatterns = new[]
        {
            // Release routes
            AppRootNoLanguage + "/{appFolder}/api/{controller}/{action}",
            AppRootNoLanguage + "/{appFolder}/{edition}/api/{controller}/{action}",
            AppRootPathOrLang + "/{appFolder}/api/{controller}/{action}",
            AppRootPathOrLang + "/{appFolder}/{edition}/api/{controller}/{action}",
            AppRootPathNdLang + "/{appFolder}/api/{controller}/{action}",
            AppRootPathNdLang + "/{appFolder}/{edition}/api/{controller}/{action}",
            // Beta routes
            WebApiStateRoot + "/app/{appFolder}/api/{controller}/{action}",
            WebApiStateRoot + "/app/{appFolder}/{edition}/api/{controller}/{action}"
        };

        // Regex patterns to match endpoint mappings
        public static readonly string[] SxcEndpointPathRegexPatterns = new[]
{
            // Release routes
            @"app/([\w-]*)/api/([\w-]*)/([\w-]*)",
            @"app/([\w-]*)/([\w-]*)/api/([\w-]*)/([\w-]*)",
            @"([\w-]*)/app/([\w-]*)/api/([\w-]*)/([\w-]*)",
            @"([\w-]*)/app/([\w-]*)/([\w-]*)/api/([\w-]*)/([\w-]*)",
            @"([\w-]*)/([\w-]*)/app/([\w-]*)/api/([\w-]*)/([\w-]*)",
            @"([\w-]*)/([\w-]*)/app/([\w-]*)/([\w-]*)/api/([\w-]*)/([\w-]*)",
            // Beta routes
            @"(\d+)/api/sxc/app/([\w-]*)/api/([\w-]*)/([\w-]*)",
            @"(\d+)/api/sxc/app/([\w-]*)/([\w-]*)/api/([\w-]*)/([\w-]*)"
        };

        // Dialogs for 2sxc UI
        public static readonly List<(string url, string page, EditUiResourceSettings setting)> SxcFallbacks = new(capacity: 2)
        {
            ($"/Modules/{OqtConstants.PackageName}/dist/quickDialog/", $@"Modules\{OqtConstants.PackageName}\dist\quickDialog\index-raw.html", EditUiResourceSettings.QuickDialog),
            ($"/Modules/{OqtConstants.PackageName}/dist/ng-edit/", $@"Modules\{OqtConstants.PackageName}\dist\ng-edit\index-raw.html", EditUiResourceSettings.EditUi)
        };
    }
}
