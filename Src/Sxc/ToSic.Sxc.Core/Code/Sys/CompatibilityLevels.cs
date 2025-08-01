namespace ToSic.Sxc.Code.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CompatibilityLevels
{
    internal const int CompatibilityLevel9Old = 9;

    /// <summary>
    /// This enforces certain features to go away or appear, like
    /// - Off: DynamicEntity.Render
    /// </summary>
    public const int CompatibilityLevel10 = 10;

    public const int CompatibilityLevel12 = 12;

    /// <summary>
    /// Typed code
    /// </summary>
    public const int CompatibilityLevel16 = 16;

    /// <summary>
    /// Old, probably remove EOY 2025
    /// </summary>
    public const int MaxLevelForAutoJQuery = CompatibilityLevel9Old;
    public const int MaxLevelForEntityDotToolbar = CompatibilityLevel9Old;
    public const int MaxLevelForEntityDotRender = CompatibilityLevel9Old;

    public const int MinLevelForTyped = CompatibilityLevel16;

    //public const int MaxLevelForStaticRender = CompatibilityLevel10;
}