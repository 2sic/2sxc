using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        [PrivateApi]
        protected override object _getValue(string field, string language = null, bool lookup = true)
        {
            #region check the two special cases Toolbar / Presentation which the EAV doesn't know
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
#pragma warning disable 618
            if (field == "Toolbar") return Toolbar.ToString();
#pragma warning restore 618
#endif
            if (field == ViewParts.Presentation) return Presentation;

            #endregion

            return base._getValue(field, language, lookup);

            //// This determines if we should access & store in cache
            //var defaultMode = language == null && lookup;
            
            //// use the standard dimensions or overload
            //var dimsToUse = language == null ? _Dependencies.Dimensions : new[] { language };

            //// check if we already have it in the cache - but only in default languages
            //if (defaultMode && _ValueCache.ContainsKey(field)) return _ValueCache[field];

            //var resultSet = _getValueRaw(field, dimsToUse);// Entity.ValueAndType(field, dimsToUse);
            
            //// check Entity is null (in cases where null-objects are asked for properties)
            //if (resultSet == null) return null;
            
            //var result = ValueAutoConverted(resultSet.Item1, resultSet.Item2, lookup, resultSet.Item3, field);

            //// cache result, but only if using default languages
            //if (defaultMode) _ValueCache.Add(field, result);
            //return result;
        }

        protected override Tuple<object, string, IEntity, string> _getValueRaw(string field, string[] dimensions)
        {
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return null;
            var t = Entity.ValueAndType(field, dimensions);
            return new Tuple<object, string, IEntity, string>(t.Item1, t.Item2, Entity, "default");
        }


        ///// <summary>
        ///// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        ///// </summary>
        ///// <param name="name">the property name. </param>
        ///// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        //public dynamic Get(string name) => _getValue(name);


        ///// <inheritdoc/>
        //public dynamic Get(string name,
        //    // ReSharper disable once MethodOverloadWithOptionalParameter
        //    string dontRelyOnParameterOrder = Eav.Parameters.Protector,
        //    string language = null,
        //    bool convertLinks = true)
        //{
        //    Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Get",
        //        $"{nameof(language)}, {nameof(convertLinks)}");
        //    return _getValue(name, language, convertLinks);
        //}
    }
}
