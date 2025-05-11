namespace ToSic.Sxc.Services.Tweaks;

internal class TweakConfigs
{
    public TweakConfigs(TweakConfig tweak)
    {
        List = [tweak];
    }

    public TweakConfigs(TweakConfigs original, TweakConfig additional = default) : this (original?.List, additional)
    {
    }

    private TweakConfigs(IEnumerable<TweakConfig> tweaks, TweakConfig additional = default)
    {
        List = tweaks?.ToList() ?? [];
        if (additional != default) List.Add(additional);
    }

    public List<TweakConfig> List { get; }

    internal List<TweakConfig> GetTweaksByName(string nameId)
        => List.Where(t => t.NameId == nameId).ToList();

    internal List<TweakConfig> GetTweaksByStep(string step)
        => List.Where(t => t.Step == step).ToList();

}