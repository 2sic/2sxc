using System;
using System.Collections.Generic;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {

        private object _getValue(string field, string language = null, bool lookup = true)
        {
            var defaultMode = language == null && lookup;

            #region check the two special cases Toolbar / Presentation which the EAV doesn't know
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
#pragma warning disable 618
            if (field == "Toolbar") return Toolbar.ToString();
#pragma warning restore 618
#endif
            if (field == ViewParts.Presentation) return Presentation;

            #endregion

            // use the standard dimensions or overload
            var dimsToUse = language == null ? _Dependencies.Dimensions : new[] { language };

            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return null;

            // check if we already have it in the cache - but only in normal lookups
            if (defaultMode && _ValueCache.ContainsKey(field)) return _ValueCache[field];

            var resultSet = Entity.ValueAndType(field, dimsToUse);
            var result = ValueAutoConverted(resultSet.Item1, resultSet.Item2, lookup, Entity, field);

            if (defaultMode) _ValueCache.Add(field, result);
            return result;
        }

        //private readonly Dictionary<string, object> _valCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);


        
        
        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name">the property name. </param>
        /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        public dynamic Get(string name) => _getValue(name);


        /// <inheritdoc/>
        public dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Get",
                $"{nameof(language)}, ... (optional)");
            return _getValue(name, language, convertLinks);
        }
    }
}
