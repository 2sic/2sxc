namespace ToSic.Sxc.Images.Sys;

/// <summary>
/// Interface for ImageDecorator,
/// so we can also use it when not created from an IEntity.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IImageDecorator
{
    bool? LightboxIsEnabled { get; }
    string? LightboxGroup { get; }
    string? DescriptionExtended { get; }
}