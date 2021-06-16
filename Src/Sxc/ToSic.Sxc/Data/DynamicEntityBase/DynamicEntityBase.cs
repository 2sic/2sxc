using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    public abstract class DynamicEntityBase: DynamicObject, IDynamicEntityBase, IPropertyLookup
    {
        protected DynamicEntityBase(DynamicEntityDependencies dependencies) => _Dependencies = dependencies;

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public DynamicEntityDependencies _Dependencies { get; }

        // ReSharper disable once InconsistentNaming
        protected readonly Dictionary<string, object> _ValueCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public void SetDebug(bool debug) => _debug = debug;
        protected bool _debug;
        
        [PrivateApi]
        protected virtual object GetInternal(string field, string language = null, bool lookup = true)
        {
            var log = _debug ? _Dependencies.LogOrNull : null;
            var safeWrap = log.SafeCall<object>($"{nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookup)}:{lookup}", "Debug active");
            // This determines if we should access & store in cache
            var defaultMode = language == null && lookup;

            // use the standard dimensions or overload
            var languages = language == null ? _Dependencies.Dimensions : new[] { language };
            log.SafeAdd($"{nameof(defaultMode)}: {defaultMode}, {nameof(languages)}:{languages}");

            // check if we already have it in the cache - but only in default languages
            if (defaultMode && _ValueCache.ContainsKey(field))
                return safeWrap("cached", _ValueCache[field]);

            var resultSet = FindPropertyInternal(field, languages, log);

            // check Entity is null (in cases where null-objects are asked for properties)
            if (resultSet == null) 
                return safeWrap("null", null);

            // Log more details
            log.SafeAdd($"Result... IsFinal: {resultSet.IsFinal}, Source Name: {resultSet.Name}, SourceIndex: {resultSet.SourceIndex}, FieldType: {resultSet.FieldType}");

            var result = ValueAutoConverted(resultSet, lookup, field);

            // cache result, but only if using default languages
            if (defaultMode)
            {
                log.SafeAdd("add to cache");
                _ValueCache.Add(field, result);
            }
            return safeWrap("ok", result);
        }

        [PrivateApi("Internal")]
        public abstract PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull);
        
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


        protected object ValueAutoConverted(PropertyRequest original, bool lookup, string field)
        {
            var log = _debug ? _Dependencies.LogOrNull : null;
            var safeWrap = log.SafeCall<object>($"..., {nameof(lookup)}: {lookup}, {nameof(field)}: {field}");
            var result = original.Result;
            var parent = original.Source as IEntity;
            // New mechanism to not use resolve-hyperlink
            if (lookup && original.Result is string strResult
                       && original.FieldType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
            {
                log.SafeAdd($"Try to convert value - HasValueConverter: {_Dependencies.ValueConverterOrNull != null}");
                result = _Dependencies.ValueConverterOrNull?.ToValue(strResult, parent?.EntityGuid ?? Guid.Empty) ?? result;
                return safeWrap(null, result);
            }

            // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
            // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
            if (result is IEnumerable<IEntity> children)
            {
                log.SafeAdd($"Convert entity list to {nameof(DynamicEntityWithList)}");
                result = new DynamicEntityWithList(parent, field, children, _Dependencies);
            }
            
            return safeWrap(null, result);
        }

        
        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        [PrivateApi]
        protected IDynamicEntity SubDynEntity(IEntity contents) => SubDynEntity(contents, _Dependencies);

        internal static IDynamicEntity SubDynEntity(IEntity contents, DynamicEntityDependencies dependencies) 
            => contents == null ? null : new DynamicEntity(contents, dependencies);
    }
}
