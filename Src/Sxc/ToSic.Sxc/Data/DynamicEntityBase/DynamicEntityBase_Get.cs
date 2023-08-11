using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase: ICanGetByName
    {
        #region Get / Get<T>

        /// <inheritdoc/>
        public dynamic Get(string name) => Helper.GetInternal(name, lookupLink: true).Result;

        /// <inheritdoc/>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public dynamic Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null)
        {
            Protect(noParamOrder, $"{nameof(language)}, {nameof(convertLinks)}");

            var debugBefore = Debug;
            if (debug != null) Debug = debug.Value;
            var result = Helper.GetInternal(name, language, convertLinks).Result;
            if (debug != null) Debug = debugBefore;

            return result;
        }

        /// <inheritdoc/>
        public TValue Get<TValue>(string name) => Helper.TryGet(name).Result.ConvertOrDefault<TValue>();

        /// <inheritdoc/>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            return Helper.TryGet(name).Result.ConvertOrFallback(fallback);
        }

        #endregion
    }
}
