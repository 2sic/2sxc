using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    public abstract class DynamicEntityBase: DynamicObject, IDynamicEntityGet
    {
        protected DynamicEntityBase(DynamicEntityDependencies dependencies) => _Dependencies = dependencies;

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public DynamicEntityDependencies _Dependencies { get; }

        // ReSharper disable once InconsistentNaming
        protected readonly Dictionary<string, object> _ValueCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        [PrivateApi]
        protected virtual object _getValue(string field, string language = null, bool lookup = true)
        {
            // This determines if we should access & store in cache
            var defaultMode = language == null && lookup;

            // use the standard dimensions or overload
            var dimsToUse = language == null ? _Dependencies.Dimensions : new[] { language };

            // check if we already have it in the cache - but only in default languages
            if (defaultMode && _ValueCache.ContainsKey(field)) return _ValueCache[field];

            var resultSet = _getValueRaw(field, dimsToUse);// Entity.ValueAndType(field, dimsToUse);

            // check Entity is null (in cases where null-objects are asked for properties)
            if (resultSet == null) return null;

            var result = ValueAutoConverted(resultSet.Item1, resultSet.Item2, lookup, resultSet.Item3, field);

            // cache result, but only if using default languages
            if (defaultMode) _ValueCache.Add(field, result);
            return result;
        }

        [PrivateApi]
        protected abstract Tuple<object, string, IEntity, string> _getValueRaw(string field, string[] dimensions);
        
        /// <inheritdoc/>
        public dynamic Get(string name) => _getValue(name);

        /// <inheritdoc/>
        public dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Get",
                $"{nameof(language)}, {nameof(convertLinks)}");
            return _getValue(name, language, convertLinks);
        }


        protected object ValueAutoConverted(object result, string dataType, bool lookup, IEntity entity, string field)
        {
            // New mechanism to not use resolve-hyperlink
            if (lookup && result is string strResult
                       && dataType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
                result = _Dependencies.ValueConverterOrNull?.ToValue(strResult, entity.EntityGuid) ?? result;

            // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
            return result is IEnumerable<IEntity> children
                ? new DynamicEntityWithList(entity, field, children, _Dependencies)
                : result;
        }

    }
}
