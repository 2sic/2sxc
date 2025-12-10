namespace ToSic.Sxc.Services.Tweaks.Sys;

internal class TweakInput<TInput>
    : ITweakInput<TInput>
{
    public List<TweakConfig> Tweaks { get; init; } = [];

    [PublicApi]
    public ITweakInput<TInput> Input(TInput replace, NoParamOrder npo = default)
        => CloneWith(_ => replace);

    [PublicApi]
    public ITweakInput<TInput> Input(Func<TInput> func, NoParamOrder npo = default)
        => CloneWith(_ => func());

    [PublicApi]
    public ITweakInput<TInput> Input(Func<TInput, TInput> func, NoParamOrder npo = default)
        => CloneWith(tv => func(tv.Value!));

    /// <summary>
    /// Create new TweakInput with added tweak
    /// </summary>
    /// <returns></returns>
    internal TweakInput<TInput> CloneWith(Func<ITweakData<TInput>, TInput> changeFunc, string? nameId = default, string? step = default, string? target = default)
        => new()
        {
            // Create new tweak config to add
            Tweaks = Tweaks.CloneAndAddNonNull(TweakConfigWithFunction.CreateTweak(changeFunc, nameId, step, target))
        };
}