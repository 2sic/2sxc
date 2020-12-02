using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Adam;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Adam
{
    public static class Configuration
    {
        public const string TypeName = "AdamConfiguration";

        public const string ItemFolderMask = "[AdamRoot]/[Guid22]/[FieldName]/[SubFolder]";


        public static string AdamAppRootFolder =>
            _adamAppRootFolder ?? (_adamAppRootFolder =
                Global.GetOverride(TypeName, "AppRootFolder",
                    $"{AdamConstants.AdamRootFolder}/[AppFolder]/"));

        private static string _adamAppRootFolder;

        internal static Dictionary<string, string> AppReplacementMap(AppState app) => new Dictionary<string, string>
        {
            {"[AppFolder]", app.Folder},
            {"[ZoneId]", app.ZoneId.ToString()},
            {"[AppId]", app.AppId.ToString()},
            {"[AppGuid]", app.AppGuidName}
        };

        internal static string ReplaceInsensitive(this Dictionary<string, string> list, string mask)
            => list.Aggregate(mask, (current, dicItem)
                => Regex.Replace(current, Regex.Escape(dicItem.Key), dicItem.Value, RegexOptions.CultureInvariant));
    }
}
