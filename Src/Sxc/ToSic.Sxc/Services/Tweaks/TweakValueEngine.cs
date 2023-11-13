using System;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Services.Tweaks
{
    public class TweakValueEngine
    {
        public TValue MaybeTweakValue<TValue>(TValue value, Func<ITweakValue<TValue>, ITweakValue<TValue>> func, ILog log)
        {
            var l = log.Fn<TValue>();
            if (func == null) return l.Return(value, "no tweak");
            try
            {
                var result = func(new TweakValue<TValue>("Default", "Default", value));
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
}
