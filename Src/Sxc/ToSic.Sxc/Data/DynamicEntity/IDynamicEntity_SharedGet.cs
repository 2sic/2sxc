using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial interface IDynamicEntity
    {
        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        /// <inheritdoc cref="IDynamicEntityDocs.Get(string)"/>
#if NETFRAMEWORK
        new
#endif
            dynamic Get(string name);


        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        /// <inheritdoc cref="IDynamicEntityDocs.Get(string, string, string, bool, bool?)"/>
            // ReSharper disable once MethodOverloadWithOptionalParameter
        dynamic Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null);

        /// <inheritdoc cref="IDynamicEntityDocs.Get{TValue}(string)"/>
        TValue Get<TValue>(string name);

        /// <inheritdoc cref="IDynamicEntityDocs.Get{TValue}(string, string, TValue)"/>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default);

    }
}
