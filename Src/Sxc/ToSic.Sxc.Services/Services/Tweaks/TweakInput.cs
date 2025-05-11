namespace ToSic.Sxc.Services.Tweaks;

internal class TweakValue
{
    public const string NameDefault = "default";
    public const string StepBefore = "before";
    public const string StepAfter = "after";
}

internal class TweakInput<TInput>(TweakInput<TInput> original = default, TweakConfig additional = default)
    : ITweakInput<TInput>
{
    public TweakConfigs Tweaks { get; } = new(original?.Tweaks, additional);

    [PublicApi]
    public ITweakInput<TInput> Input(TInput replace, NoParamOrder protector = default)
        => CloneExtend(_ => replace);

    [PublicApi]
    public ITweakInput<TInput> Input(Func<TInput> func, NoParamOrder protector = default)
        => CloneExtend(_ => func());

    [PublicApi]
    public ITweakInput<TInput> Input(Func<TInput, TInput> func, NoParamOrder protector = default)
        => CloneExtend(tv => func(tv.Value));

    [PublicApi]
    public ITweakInput<TInput> Process(Func<ITweakData<TInput>, TInput> func, NoParamOrder protector = default)
        => CloneExtend(func);

    private ITweakInput<TInput> CloneExtend(Func<ITweakData<TInput>, TInput> func, NoParamOrder protector = default, string step = default)
        => CloneExtend(func, TweakValue.NameDefault, step ?? TweakValue.StepBefore, target: TweakConfig.TargetDefault);



    private TweakInput<TInput> CloneExtend(Func<ITweakData<TInput>, TInput> changeFunc, string nameId, string step, string target)
    {
        var tweak = new TweakConfig<Func<ITweakData<TInput>, int, ITweakData<TInput>>>(nameId,
            (v, index) => new TweakData<TInput>(v, changeFunc(v), index), step, target);
        return new(this, tweak);
    }

    internal ITweakData<TInput> Preprocess(TInput html, string name = TweakValue.NameDefault) 
        => Process(html, name, TweakValue.StepBefore);

    private ITweakData<TInput> Process(TInput value, string name, string step)
    {
        var tweaks = Tweaks.GetTweaksByStep(step)
            .Select(t => t as TweakConfig<Func<ITweakData<TInput>, int, ITweakData<TInput>>>)
            .Where(t => t != null)
            .Select((tweak, id) => new { tweak, id })
            .ToList();

        var start = new TweakData<TInput>(value, name, step, 0) as ITweakData<TInput>;
        return tweaks.Aggregate(start, (current, tweak) =>
        {
            try
            {
                return tweak.tweak.Tweak(current, tweak.id);
            }
            catch (Exception e)
            {
                var exMore = new Exception($"Error in tweak #{tweak.id} '{tweak.tweak.NameId}' at step '{tweak.tweak.Step}' for target '{tweak.tweak.Target}'", e);
                throw exMore;
            }
        });
    }
}