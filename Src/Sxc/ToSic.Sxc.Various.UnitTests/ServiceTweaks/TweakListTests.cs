using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Tests.ServiceTweaks;


public class TweakListTests
{
    [Fact]
    public void CloneIsFunctional()
    {
        var tw = new TweakConfigs(null);
        var tw2 = new TweakConfigs(tw);
        NotSame(tw, tw2);
        Equal(tw.List.Count, tw2.List.Count);
        NotSame(tw.List, tw2.List);
    }

    [Fact]
    public void CloneHasSameTweaks()
    {
        var tw = new TweakConfigs(null, new("test"));
        tw = new(tw, new("test2"));
        var tw2 = new TweakConfigs(tw);
        NotSame(tw, tw2);
        Equal(tw.List.Count, tw2.List.Count);
        NotSame(tw.List, tw2.List);
        Same(tw.List[0], tw2.List[0]);//, "tweaks are the same object");
        Same(tw.List[1], tw2.List[1]);//, "tweaks are the same object");
    }
}