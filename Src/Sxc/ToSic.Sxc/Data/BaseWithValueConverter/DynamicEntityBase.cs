using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    public abstract class DynamicEntityBase: DynamicObject, IDynamicEntityGet, IPropertyLookup
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

            var resultSet = FindPropertyInternal(field, dimsToUse);

            // check Entity is null (in cases where null-objects are asked for properties)
            if (resultSet == null) return null;

            var result = ValueAutoConverted(resultSet, lookup, field);

            // cache result, but only if using default languages
            if (defaultMode) _ValueCache.Add(field, result);
            return result;
        }

        [PrivateApi("Internal")]
        public abstract PropertyRequest FindPropertyInternal(string field, string[] dimensions);
        
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


        protected object ValueAutoConverted(PropertyRequest original, bool lookup, string field)
        {
            var result = original.Result;
            var parent = original.Source as IEntity;
            // New mechanism to not use resolve-hyperlink
            if (lookup && original.Result is string strResult
                       && original.FieldType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
                result = _Dependencies.ValueConverterOrNull?.ToValue(strResult, parent?.EntityGuid ?? Guid.Empty) ?? result;

            // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
            // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
            return result is IEnumerable<IEntity> children
                ? new DynamicEntityWithList(parent, field, children, _Dependencies)
                : result;
        }

        
        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        [PrivateApi]
        protected IDynamicEntity SubDynEntity(IEntity contents) => contents == null ? null : new DynamicEntity(contents, _Dependencies);

    }
}
