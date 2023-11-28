namespace ToSic.Sxc.Oqt.Shared;

/// <summary>
/// This should only contain constants which should really be final, no WIP or similar
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class OqtConstants
{
    // used as PackageID in nupkg, oqtane module PackageName, wwwroot/Modules/PackageName, namespace in module.js, etc...
    public const string PackageName = "ToSic.Sxc.Oqtane";

    public const string SiteKeyForZoneId = "EavZone";

    public const string AppRoot = "2sxc";

    public const string AppRootPublicBase = AppRoot + "\\{0}";

    public const string SharedAppFolder =  "Shared";

    public const string ContentSubfolder = "Content";

    public const string ContentRootPublicBase = ContentSubfolder + "\\Tenants\\{0}\\Sites\\{1}";

    public const string ApiAppLinkPart = "api/sxc/app";

    public const string DownloadLinkTemplate = "/{0}/api/file/download/{1}";

    public const string UserTokenPrefix = "oqt:userid=";

    // #uncertain: maybe should incorporate the virtual path of the application?
    public const string UiRoot = $"/Modules/{OqtConstants.PackageName}";
    // #uncertain: maybe should be more dynamic
    public const string SiteRoot = "/";

    // Logging constants
    public const string OqtLogPrefix = "Oqt";
        
    // Special Oqtane constants missing in Oqtane
    public const string EntityIdParam = "entityid";

    public const int Unknown = -1;
}