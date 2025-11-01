namespace ToSic.Sxc.Services.Tweaks.Sys;
internal static class TweakConfigWithFunction
{
    internal static TweakConfig<Func<ITweakData<TInput>, int, ITweakData<TInput>>>
        CreateTweak<TInput>(Func<ITweakData<TInput>, TInput> changeFunc, string? nameId = null, string? step = null, string? target = null)
        => new(nameId ?? TweakConfigConstants.NameDefault)
        {
            Step = step ?? TweakConfigConstants.StepBefore,
            Tweak = (v, index) => new TweakData<TInput>(v, changeFunc(v), index),
            Target = target ?? TweakConfig.TargetDefault,
        };

}
