namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase
    {
        /// <inheritdoc/>
        public dynamic Get(string name) => GetInternal(name);

        /// <inheritdoc/>
        public dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true,
            bool? debug = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Get",
                $"{nameof(language)}, {nameof(convertLinks)}");

            var debugBefore = _debug;
            if (debug != null) _debug = debug.Value;
            var result = GetInternal(name, language, convertLinks);
            if (debug != null) _debug = debugBefore;

            return result;
        }

    }
}
