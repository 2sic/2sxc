//using System.Text.RegularExpressions;
//using ToSic.Eav.Apps.Internal.Specs;

//namespace ToSic.Sxc.Backend.App;

//[ShowApiWhenReleased(ShowApiMode.Never)]
//public static class ExtensionsForIApp
//{
//    //public static string VersionSafe(this IAppSpecs app)
//    //    => app.Configuration.Version?.ToString() ?? "";

//    public static string NameWithoutSpecialChars(this IAppSpecs app)
//        => Regex.Replace(app.Name, "[^a-zA-Z0-9-_]", "");

//}