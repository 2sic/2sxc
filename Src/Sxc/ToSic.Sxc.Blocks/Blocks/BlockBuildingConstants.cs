﻿using ToSic.Eav.Data.Sys;

namespace ToSic.Sxc.Blocks;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockBuildingConstants
{
    public const string InputTypeForContentBlocksField = "entity-content-blocks";
    internal const string CbPropertyApp = "App";
    internal const string CbPropertyTitle = AttributeNames.TitleNiceName;
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