using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services.Tweaks;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServiceTweaks
{
    [TestClass]
    public class TweakListTests
    {
        [TestMethod]
        public void CloneIsFunctional()
        {
            var tw = new TweakConfigs(null);
            var tw2 = new TweakConfigs(tw);
            AreNotSame(tw, tw2);
            AreEqual(tw.List.Count, tw2.List.Count);
            AreNotSame(tw.List, tw2.List);
        }

        [TestMethod]
        public void CloneHasSameTweaks()
        {
            var tw = new TweakConfigs(null, new("test"));
            tw = new(tw, new("test2"));
            var tw2 = new TweakConfigs(tw);
            AreNotSame(tw, tw2);
            AreEqual(tw.List.Count, tw2.List.Count);
            AreNotSame(tw.List, tw2.List);
            AreSame(tw.List[0], tw2.List[0], "tweaks are the same object");
            AreSame(tw.List[1], tw2.List[1], "tweaks are the same object");
        }
    }
}
