namespace ToSic.Sxc.Services.Tweaks.Sys;

internal record TweakConfig<T>(string NameId)
    : TweakConfig(NameId)
{
    public required T Tweak { get; init; }


}