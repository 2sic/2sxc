namespace ToSic.Sxc.Services.Tweaks;

internal class TweakConfig<T>(string nameId, T tweak, string step = default, string target = default)
    : TweakConfig(nameId, step, target)
{
    public T Tweak { get; } = tweak;
}