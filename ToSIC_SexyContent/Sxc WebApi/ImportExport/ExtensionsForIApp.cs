using System.Text.RegularExpressions;
using ToSic.Sxc.Apps;

namespace ToSic.SexyContent.WebApi.ImportExport
{
    internal static class ExtensionsForIApp
    {
        public static string VersionSafe(this IApp app) => app.Configuration == null ? "" : app.Configuration.Version;

        public static string NameWithoutSpecialChars(this IApp app) => Regex.Replace(app.Name, "[^a-zA-Z0-9-_]", "");

    }
}
