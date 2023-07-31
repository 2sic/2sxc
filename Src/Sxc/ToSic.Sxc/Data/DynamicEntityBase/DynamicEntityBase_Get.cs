using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase: ICanGetByName
    {
        /// <inheritdoc/>
        public dynamic Get(string name) => GetInternal(name).Result;

        /// <inheritdoc/>
        public TValue Get<TValue>(string name) => GetInternal(name, lookup: false).Result.ConvertOrDefault<TValue>();

        /// <inheritdoc/>
        public TValue Get<TValue>(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Protector,
            TValue fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            return GetInternal(name, lookup: false).Result.ConvertOrFallback(fallback);
        }

        /// <inheritdoc/>
        public dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Protector,
            string language = null,
            bool convertLinks = true,
            bool? debug = null)
        {
            Protect(noParamOrder, $"{nameof(language)}, {nameof(convertLinks)}");

            var debugBefore = Debug;
            if (debug != null) Debug = debug.Value;
            var result = GetInternal(name, language, convertLinks).Result;
            if (debug != null) Debug = debugBefore;

            return result;
        }

    }
}
