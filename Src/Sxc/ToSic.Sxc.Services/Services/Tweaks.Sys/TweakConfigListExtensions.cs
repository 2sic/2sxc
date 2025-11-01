namespace ToSic.Sxc.Services.Tweaks.Sys;
internal static class TweakConfigListExtensions
{
    #region Clone Helpers - maybe centralize to ToSic.Sys some day?

    public static List<T> CloneAndAddNonNull<T>(this IEnumerable<T>? listToClone, T? additional = default)
    {
        var newList = listToClone == null ? [] : new List<T>(listToClone);
        if (additional != null)
            newList.Add(additional);
        return newList;
    }

    #endregion

    #region Get Tweaks by Step / Name

    internal static List<TweakConfig> GetTweaksByStep(this IList<TweakConfig> list, string step)
        => list.Where(t => t.Step == step).ToList();

    internal static List<TweakConfig> GetTweaksByName(this IList<TweakConfig> list, string nameId)
        => list.Where(t => t.NameId == nameId).ToList();

    #endregion

    #region Process / PreProcess

    internal static ITweakData<TInput> Preprocess<TInput>(this IList<TweakConfig> list, TInput? value, string name = TweakConfigConstants.NameDefault)
        => list.Process(value, name, TweakConfigConstants.StepBefore);

    internal static ITweakData<TInput> Process<TInput>(this IList<TweakConfig> list, TInput? value, string? name, string step)
    {
        // Find all relevant tweaks for this step
        var tweaks = list.GetTweaksByStep(step)
            .Select(t => t as TweakConfig<Func<ITweakData<TInput>, int, ITweakData<TInput>>>)
            .Where(t => t != null)
            .Select((tweak, id) => new { tweak, id })
            .ToList();

        ITweakData<TInput> start = new TweakData<TInput>(value, name, step, 0);
        return tweaks.Aggregate(start, (current, tweak) =>
        {
            try
            {
                return tweak.tweak!.Tweak(current, tweak.id);
            }
            catch (Exception e)
            {
                var exMore = new Exception($"Error in tweak #{tweak.id} '{tweak.tweak!.NameId}' at step '{tweak.tweak.Step}' for target '{tweak.tweak.Target}'", e);
                throw exMore;
            }
        });
    }

    #endregion

}
