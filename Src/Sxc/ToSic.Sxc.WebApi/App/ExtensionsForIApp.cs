using System.Text.RegularExpressions;
using ToSic.Eav.Apps;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class ExtensionsForIApp
{
    public static string VersionSafe(this IApp app) => app.Configuration.Version?.ToString() ?? "";

    public static string NameWithoutSpecialChars(this IApp app) => Regex.Replace(app.Name, "[^a-zA-Z0-9-_]", "");
    
    public static string VersionSafe(this IAppState app) => app.Configuration.Version?.ToString() ?? "";

    public static string NameWithoutSpecialChars(this IAppState app) => Regex.Replace(app.Name, "[^a-zA-Z0-9-_]", "");

}