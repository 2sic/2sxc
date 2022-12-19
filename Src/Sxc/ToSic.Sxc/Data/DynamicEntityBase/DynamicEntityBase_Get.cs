using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase: ICanGetByName
    {
        /// <inheritdoc/>
        public dynamic Get(string name) => GetInternal(name);

        [PrivateApi]
        public TValue Get<TValue>(string name) => GetInternal(name).ConvertOrDefault<TValue>();

        [PrivateApi]
        public TValue Get<TValue>(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Eav.Parameters.Protector,
            TValue fallback = default)
        {
            return GetInternal(name).ConvertOrFallback(fallback);
        }

        /// <inheritdoc/>
        public dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true,
            bool? debug = null)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(language)}, {nameof(convertLinks)}");

            var debugBefore = Debug;
            if (debug != null) Debug = debug.Value;
            var result = GetInternal(name, language, convertLinks);
            if (debug != null) Debug = debugBefore;

            return result;
        }

    }
}
