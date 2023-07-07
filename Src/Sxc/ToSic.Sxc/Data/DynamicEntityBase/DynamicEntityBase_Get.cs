using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase: ICanGetByName
    {
        /// <inheritdoc/>
        public dynamic Get(string name) => GetInternal(name);

        /// <inheritdoc/>
        public TValue Get<TValue>(string name) => GetInternal(name, lookup: false).ConvertOrDefault<TValue>();

        /// <inheritdoc/>
        public TValue Get<TValue>(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Protector,
            TValue fallback = default)
        {
            Protect(noParamOrder, $"{nameof(fallback)}");
            return GetInternal(name, lookup: false).ConvertOrFallback(fallback);
        }
        private TValue GetV<TValue>(string name,
            string noParamOrder = Protector,
            TValue fallback = default,
            [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, $"{nameof(fallback)}", methodName: cName);
            return GetInternal(name, lookup: false).ConvertOrFallback(fallback);
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
            var result = GetInternal(name, language, convertLinks);
            if (debug != null) Debug = debugBefore;

            return result;
        }

    }
}
