namespace ToSic.Sxc.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CompatibilityLevels
{
    internal const int CompatibilityLevel9Old = 9;

    /// <summary>
    /// This enforces certain features to go away or appear, like
    /// - Off: DynamiEntity.Render
    /// </summary>
    public const int CompatibilityLevel10 = 10;

    public const int CompatibilityLevel12 = 12;

    public const int CompatibilityLevel16 = 16;

    public const int MaxLevelForAutoJQuery = CompatibilityLevel9Old;
    public const int MaxLevelForEntityDotToolbar = CompatibilityLevel9Old;
    public const int MaxLevelForEntityDotRender = CompatibilityLevel9Old;

    public const int MaxLevelForStaticRender = CompatibilityLevel10;
}