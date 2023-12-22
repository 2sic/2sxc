using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services.Tweaks;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServiceTweaks
{
    [TestClass]
    public class CmsHtmlTweakTests
    {
        private (TweakInput<string> Tw1, TweakInput<string> Tw2) GetTweakers()
        {
            var tw1 = new TweakInput<string>();
            var tw2 = (TweakInput<string>)tw1.Input(v => v + "-test");
            return (tw1, tw2);
        }

        [TestMethod]
        public void AddingTweakDoesNotAffectOriginal()
        {
            var (tw1, tw2) = GetTweakers();
            AreEqual(0, tw1.Tweaks.List.Count);
            AreEqual(1, tw2.Tweaks.List.Count);
        }

        [TestMethod]
        public void AddingTweakIsCorrectType()
        {
            var (_, tw2) = GetTweakers();
            AreEqual(TweakValue.NameDefault, tw2.Tweaks.List[0].NameId);
            AreEqual(TweakValue.StepBefore, tw2.Tweaks.List[0].Step);
        }

        [TestMethod]
        public void GetPreprocessIsCorrect()
        {
            var (_, tw2) = GetTweakers();
            var preprocess = tw2.Tweaks.GetTweaksByStep(TweakValue.StepBefore);
            AreEqual(1, preprocess.Count);
            AreEqual(TweakValue.NameDefault, preprocess[0].NameId);
            AreEqual(TweakValue.StepBefore, tw2.Tweaks.List[0].Step);
        }

        [TestMethod]
        public void GetPostProcessIsCorrect()
        {
            var (_, tw2) = GetTweakers();
            var preprocess = tw2.Tweaks.GetTweaksByStep(TweakValue.StepAfter);
            AreEqual(0, preprocess.Count);
        }

        [TestMethod]
        public void Preprocess()
        {
            var (_, tw2) = GetTweakers();

            var processed = tw2.Preprocess("Hello");
            AreEqual("Hello-test", processed.Value);
        }
    }
}
