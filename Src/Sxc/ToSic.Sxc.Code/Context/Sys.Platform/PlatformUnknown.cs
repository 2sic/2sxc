namespace ToSic.Sxc.Context.Sys.Platform;

// ReSharper disable UnusedParameter.Local

internal class PlatformUnknown(WarnUseOfUnknown<PlatformUnknown> _): Platform, IIsUnknown
{
    public override PlatformType Type => PlatformType.Unknown;

    public override Version Version => new(0, 0, 0);
}