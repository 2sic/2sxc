namespace ToSic.Sxc.Images.Internal;

/// <summary>
/// Interface for ImageDecorator,
/// so we can also use it when not created from an IEntity.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal interface IImageDecorator
{
    bool? LightboxIsEnabled { get; }
    string LightboxGroup { get; }
    string DescriptionExtended { get; }
}