using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Tests.ServiceTweaks;


public class CmsHtmlTweakTests
{
    private (TweakInput<string> Tw1, TweakInput<string> Tw2) GetTweakers()
    {
        var tw1 = new TweakInput<string>();
        var tw2 = (TweakInput<string>)tw1.Input(v => v + "-test");
        return (tw1, tw2);
    }

    [Fact]
    public void AddingTweakDoesNotAffectOriginal()
    {
        var (tw1, tw2) = GetTweakers();
        Equal(0, tw1.Tweaks.List.Count);
        Equal(1, tw2.Tweaks.List.Count);
    }

    [Fact]
    public void AddingTweakIsCorrectType()
    {
        var (_, tw2) = GetTweakers();
        Equal(TweakValue.NameDefault, tw2.Tweaks.List[0].NameId);
        Equal(TweakValue.StepBefore, tw2.Tweaks.List[0].Step);
    }

    [Fact]
    public void GetPreprocessIsCorrect()
    {
        var (_, tw2) = GetTweakers();
        var preprocess = tw2.Tweaks.GetTweaksByStep(TweakValue.StepBefore);
        Equal(1, preprocess.Count);
        Equal(TweakValue.NameDefault, preprocess[0].NameId);
        Equal(TweakValue.StepBefore, tw2.Tweaks.List[0].Step);
    }

    [Fact]
    public void GetPostProcessIsCorrect()
    {
        var (_, tw2) = GetTweakers();
        var preprocess = tw2.Tweaks.GetTweaksByStep(TweakValue.StepAfter);
        Equal(0, preprocess.Count);
    }

    [Fact]
    public void Preprocess()
    {
        var (_, tw2) = GetTweakers();

        var processed = tw2.Preprocess("Hello");
        Equal("Hello-test", processed.Value);
    }
}