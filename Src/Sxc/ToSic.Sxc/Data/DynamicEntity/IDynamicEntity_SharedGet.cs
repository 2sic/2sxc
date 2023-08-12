namespace ToSic.Sxc.Data
{
    public partial interface IDynamicEntity
    {
        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        /// <inheritdoc cref="IDynamicEntityBase.Get(string)"/>
        new dynamic Get(string name);


        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        /// <inheritdoc cref="IDynamicEntityBase.Get(string, string, string, bool, bool?)"/>
        new dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true,
            bool? debug = null
        );
    }
}
