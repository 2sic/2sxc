namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockBuildingConstants
{
    internal const string InputTypeForContentBlocksField = "entity-content-blocks";
    internal const string CbPropertyApp = "App";
    internal const string CbPropertyTitle = Attributes.TitleNiceName;
    internal const string CbPropertyContentGroup = "ContentGroup";

    #region Error Messages

    public static string ErrorInstallationNotOk = "InstallationNotOk";

    public static string ErrorDataIsMissing = "DataIsMissing";

    public static string ErrorRendering = "Rendering";

    public static string ErrorGeneral = "General";

    public static string ErrorHtmlMarker = "<!--2sxc:error-->";

    public static string ErrorAppIsUnhealthy = "AppIsUnhealthy";
    
    public static string AppIsUnhealthy = "2sxc app is unhealthy. ";

    #endregion
}