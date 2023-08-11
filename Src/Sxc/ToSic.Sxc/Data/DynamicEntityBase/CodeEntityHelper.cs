using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data.Decorators;
using static System.StringComparer;

namespace ToSic.Sxc.Data
{
    internal class CodeEntityHelper
    {

        #region Setup and Log

        public CodeEntityHelper(IPropertyLookup parent, CodeDataFactory cdf, bool strict)
        {
            Parent = parent;
            Cdf = cdf;
            StrictGet = strict;
        }

        public CodeDataFactory Cdf { get; }

        public bool StrictGet { get; }


        public readonly IPropertyLookup Parent;

        public bool Debug { get; set; }

        public ILog LogOrNull => _logOrNull.Get(() => Cdf?.Log?.SubLogOrNull("DynEnt", Debug));
        private readonly GetOnce<ILog> _logOrNull = new GetOnce<ILog>();

        #endregion

        #region Get Values

        public TryGetResult TryGet(string field, string language = null) => GetInternal(field, language, lookupLink: false);

        public TryGetResult GetInternal(string field, string language = null, bool lookupLink = true)
        {
            var logOrNull = LogOrNull.SubLogOrNull("Dyn.EntBas", Debug);
            var l = logOrNull.Fn<TryGetResult>($"Type: {GetType().Name}, {nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookupLink)}:{lookupLink}");

            if (!field.HasValue())
                return l.Return(new TryGetResult(false, null), "field null/empty");

            // This determines if we should access & store in cache
            // check if we already have it in the cache - but only in default case (no language, lookup=true)
            var cacheKey = (field + "$" + lookupLink + "-" + language).ToLowerInvariant();
            if (_rawValCache.TryGetValue(cacheKey, out var cached))
                return l.Return(cached, "cached");

            // use the standard dimensions or overload
            var languages = language == null ? Cdf.Dimensions : new[] { language };
            l.A($"cache-key: {cacheKey}, {nameof(languages)}:{languages}");

            // Get the field or the path if it has one
            // Classic field case
            var specs = new PropReqSpecs(field, languages, logOrNull);
            var path = new PropertyLookupPath().Add("DynEntStart", field);
            var resultSet = Parent.FindPropertyInternal(specs, path);

            // check Entity is null (in cases where null-objects are asked for properties)
            if (resultSet == null)
                return l.Return(new TryGetResult(false, null), "result null");

            l.A($"Result... IsFinal: {resultSet.IsFinal}, Source Name: {resultSet.Name}, SourceIndex: {resultSet.SourceIndex}, FieldType: {resultSet.FieldType}");

            var result = ValueAutoConverted(resultSet, lookupLink, field, logOrNull);

            // cache result, but only if using default languages
            l.A("add to cache");
            var found = resultSet.FieldType != Attributes.FieldIsNotFound;
            var final = new TryGetResult(found, result);
            if (found)
                _rawValCache.Add(cacheKey, final);

            return l.Return(final, "ok");
        }
        private readonly Dictionary<string, TryGetResult> _rawValCache = new Dictionary<string, TryGetResult>(InvariantCultureIgnoreCase);

        //public string StringAsPossibleLinkOrNull(PropReqResult original, ILog logOrNull)
        //{
        //    // If not string, then it can't be a link - otherwise continue, as in every case this is the only conversion
        //    if (!(original.Result is string value)) 
        //        return null;
        //    var l = logOrNull.Fn<string>();
        //    var parent = original.Source as IEntity;
        //    // New mechanism to not use resolve-hyperlink
        //    if (original.FieldType == DataTypes.Hyperlink && ValueConverterBase.CouldBeReference(value))
        //    {
        //        l.A($"Try to convert value - HasValueConverter: {Cdf.Services.ValueConverterOrNull != null}");
        //        value = Cdf.Services.ValueConverterOrNull?.ToValue(value, parent?.EntityGuid ?? Guid.Empty) ?? value;
        //        return l.Return(value, "link-conversion");
        //    }
        //    return l.Return(value, "no conversion");
        //}

        public object ValueAutoConverted(PropReqResult original, bool lookupLink, string field, ILog logOrNull)
        {
            var l = logOrNull.Fn<object>($"..., {nameof(lookupLink)}: {lookupLink}, {nameof(field)}: {field}");
            var value = original.Result;
            var parent = original.Source as IEntity;
            // New mechanism to not use resolve-hyperlink
            if (lookupLink && value is string strResult
                       && original.FieldType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
            {
                l.A($"Try to convert value - HasValueConverter: {Cdf.Services.ValueConverterOrNull != null}");
                value = Cdf.Services.ValueConverterOrNull?.ToValue(strResult, parent?.EntityGuid ?? Guid.Empty) ?? value;
                return l.Return(value, "link-conversion");
            }

            // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
            // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
            if (value is IEnumerable<IEntity> children)
            {
                l.A($"Convert entity list as {nameof(DynamicEntity)}");
                var dynEnt = new DynamicEntity(children.ToArray(), parent, field, null, strict: StrictGet, Cdf);
                if (Debug) dynEnt.Debug = true;
                return l.Return(dynEnt, "ent-list, now dyn");
            }

            // special debug of path if possible
            try
            {
                var finalPath = string.Join(" > ", original.Path?.Parts?.ToArray() ?? Array.Empty<string>());
                l.A($"Debug path: {finalPath}");
            }
            catch {/* ignore */}

            return l.Return(value, "unmodified");
        }

        #endregion

        #region Create Entities

        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public IDynamicEntity SubDynEntityOrNull(IEntity contents) => SubDynEntityOrNull(contents, Cdf, Debug, strictGet: StrictGet);

        internal static IDynamicEntity SubDynEntityOrNull(IEntity contents, CodeDataFactory cdf, bool? debug, bool strictGet)
        {
            if (contents == null) return null;
            var result = new DynamicEntity(contents, cdf, strict: strictGet);
            if (debug == true) result.Debug = true;
            return result;
        }

        public IEntity PlaceHolder(int? appIdOrNull, IEntity parent, string field)
        {
            var dummyEntity = Cdf.FakeEntity(appIdOrNull ?? parent.AppId);
            return parent == null ? dummyEntity : EntityInBlockDecorator.Wrap(dummyEntity, parent.EntityGuid, field);
        }


        #endregion

    }
}
