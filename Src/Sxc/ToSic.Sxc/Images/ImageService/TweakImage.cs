using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Images;

internal record TweakImage(ImageDecoratorVirtual VDec = default): ITweakImage
{
    public ITweakImage LightboxEnable(bool isEnabled = true) => new TweakImage(VDec with { LightboxIsEnabled = isEnabled });

    public ITweakImage LightboxGroup(string group) => new TweakImage(VDec with { LightboxGroup = group });

    public ITweakImage LightboxDescription(string description) => new TweakImage(VDec with { DescriptionExtended = description });
}

internal record ImageDecoratorVirtual(bool? LightboxIsEnabled, string LightboxGroup, string DescriptionExtended) : IImageDecorator;