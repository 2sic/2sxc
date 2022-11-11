using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntityBase
    {
        [PrivateApi]
        protected virtual object GetInternal(string field, string language = null, bool lookup = true)
        {
            var logOrNull = _Dependencies.LogOrNull.SubLogOrNull("Dyn.EntBas", Debug);
            var safeWrap = logOrNull.Fn<object>(Debug,
                    $"Type: {GetType().Name}, {nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookup)}:{lookup}",
                    "Debug: true");
            
            // This determines if we should access & store in cache
            var useCache = language == null && lookup;

            // use the standard dimensions or overload
            var languages = language == null ? _Dependencies.Dimensions : new[] { language };
            safeWrap.A($"{nameof(useCache)}: {useCache}, {nameof(languages)}:{languages}");

            // check if we already have it in the cache - but only in default languages
            if (useCache && _ValueCache.ContainsKey(field)) return safeWrap.Return(_ValueCache[field], "cached");

            var resultSet = FindPropertyInternal(field, languages, logOrNull, new PropertyLookupPath().Add("DynEntStart", field));

            // check Entity is null (in cases where null-objects are asked for properties)
            if (resultSet == null) return safeWrap.ReturnNull("null");

            safeWrap.A($"Result... IsFinal: {resultSet.IsFinal}, Source Name: {resultSet.Name}, SourceIndex: {resultSet.SourceIndex}, FieldType: {resultSet.FieldType}");

            var result = ValueAutoConverted(resultSet, lookup, field, logOrNull);

            // cache result, but only if using default languages
            if (useCache)
            {
                safeWrap.A("add to cache");
                _ValueCache.Add(field, result);
            }
            return safeWrap.ReturnAsOk(result);
        }


        protected object ValueAutoConverted(PropertyRequest original, bool lookup, string field, ILog logOrNull)
        {
            var safeWrap = logOrNull.Fn<object>($"..., {nameof(lookup)}: {lookup}, {nameof(field)}: {field}");
            var result = original.Result;
            var parent = original.Source as IEntity;
            // New mechanism to not use resolve-hyperlink
            if (lookup && original.Result is string strResult
                       && original.FieldType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
            {
                safeWrap.A($"Try to convert value - HasValueConverter: {_Dependencies.ValueConverterOrNull != null}");
                result = _Dependencies.ValueConverterOrNull?.ToValue(strResult, parent?.EntityGuid ?? Guid.Empty) ?? result;
                return safeWrap.Return(result, "link-conversion");
            }

            // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
            // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
            if (result is IEnumerable<IEntity> children)
            {
                safeWrap.A($"Convert entity list as {nameof(DynamicEntity)}");
                var dynEnt = new DynamicEntity(children.ToArray(), parent, field, null, _Dependencies);
                if (Debug) dynEnt.Debug = true;
                return safeWrap.Return(dynEnt, "ent-list, now dyn");
            }

            // special debug of path if possible
            try
            {
                var finalPath = string.Join(" > ", original.Path?.Parts?.ToArray() ?? Array.Empty<string>());
                safeWrap.A($"Debug path: {finalPath}");
            }
            catch {/* ignore */}

            return safeWrap.Return(result, "unmodified");
        }

    }
}
