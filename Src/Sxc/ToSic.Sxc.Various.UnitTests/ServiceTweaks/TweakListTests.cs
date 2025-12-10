using ToSic.Sxc.Services.Tweaks;
using ToSic.Sxc.Services.Tweaks.Sys;

namespace ToSic.Sxc.Tests.ServiceTweaks;


public class TweakListTests
{
    [Fact]
    public void CloneIsFunctional()
    {
        var tw = new TweakConfig("dummy-name");
        var tw2 = tw with { };
        NotSame(tw, tw2);
        //Equal(tw.List.Count, tw2.List.Count);
        //NotSame(tw.List, tw2.List);
    }

    //[Fact]
    //public void CloneHasSameTweaks()
    //{
    //    var tw = new TweakConfig<>(null) with { Tweak = new((string)"test") };
    //    tw = new((TweakConfig?)tw, new((string)"test2"));
    //    var tw2 = new TweakConfig(tw);
    //    NotSame(tw, tw2);
    //    Equal(tw.List.Count, tw2.List.Count);
    //    NotSame(tw.List, tw2.List);
    //    Same(tw.List[0], tw2.List[0]);//, "tweaks are the same object");
    //    Same(tw.List[1], tw2.List[1]);//, "tweaks are the same object");
    //}
}