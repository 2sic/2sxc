namespace ToSic.Sxc.Images;

public interface IResizeSettingsInternal
{
    /// <summary>
    /// Settings which are used when img/picture tags are generated with multiple resizes
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP - will move to public once official, and then probably an IAdvancedSettings")]
    AdvancedSettings? Advanced { get; }
}