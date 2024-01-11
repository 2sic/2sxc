namespace ToSic.Sxc.Services.Tweaks;

/// <summary>
/// Not in use yet
/// </summary>
internal class TweakValueEngine
{
    public TValue MaybeTweakValue<TValue>(TValue value, Func<ITweakData<TValue>, ITweakData<TValue>> func, ILog log)
    {
        var l = log.Fn<TValue>();
        if (func == null) return l.Return(value, "no tweak");
        try
        {
            var result = func(new TweakData<TValue>(value, "Default", "Default", 0));
            if (result == null) return l.Return(value, "no tweak result, keep original");
            var isChanged = value?.Equals(result.Value) ?? (value == null && result.Value == null);
            return l.Return(result.Value, $"tweaked; changed: {isChanged}");
        }
        catch (Exception e)
        {
            l.Ex(e);
            return l.Return(value, "error, preserve original");
        }
    }
}