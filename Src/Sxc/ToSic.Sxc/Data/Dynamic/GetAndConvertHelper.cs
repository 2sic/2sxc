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
using static ToSic.Eav.Parameters;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    internal class GetAndConvertHelper
    {
        #region Setup and Log


        public CodeDataFactory Cdf { get; }

        public bool StrictGet { get; }

        public GetAndConvertHelper(IHasPropLookup parent, CodeDataFactory cdf, bool strict, Func<bool> getDebug, bool childrenShouldBeDynamic)
        {
            _getDebug = getDebug;
            _childrenShouldBeDynamic = childrenShouldBeDynamic;
            Parent = parent;
            Cdf = cdf;
            StrictGet = strict;
        }

        public bool Debug => _debug ?? _getDebug();
        private bool? _debug;
        private readonly Func<bool> _getDebug;
        private readonly bool _childrenShouldBeDynamic;

        internal SubDataFactory SubDataFactory => _subData ?? (_subData = new SubDataFactory(Cdf, StrictGet, Debug));
        private SubDataFactory _subData;


        public IHasPropLookup Parent { get; }

        public ILog LogOrNull => _logOrNull.Get(() => Cdf?.Log?.SubLogOrNull("DynEnt", Debug));
        private readonly GetOnce<ILog> _logOrNull = new GetOnce<ILog>();

        #endregion

        #region Get Implementations 1:1 - names must be identical with caller, so the exceptions have the right names

        public dynamic Get(string name) => GetInternal(name, lookupLink: true).Result;

        public object Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null)
        {
            Protect(noParamOrder, $"{nameof(language)}, {nameof(convertLinks)}");

            _debug = debug;
            var result = GetInternal(name, language, convertLinks).Result;
            _debug = null;

            return result;
        }

        public TValue Get<TValue>(string name) => TryGet(name).Result.ConvertOrDefault<TValue>();

        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            return TryGet(name).Result.ConvertOrFallback(fallback);
        }


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
            var resultSet = Parent.PropertyLookup.FindPropertyInternal(specs, path);

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
                if (_childrenShouldBeDynamic)
                {
                    l.A($"Convert entity list as {nameof(DynamicEntity)}");
                    var dynEnt = new DynamicEntity(children.ToArray(), parent, field, null, strict: StrictGet, Cdf);
                    if (Debug) dynEnt.Debug = true;
                    return l.Return(dynEnt, "ent-list, now dyn");
                }
                l.A($"Convert entity list as {nameof(ITypedItem)}");
                var converted = Entity2Children(children, parent?.EntityGuid, field)
                    .Cast<DynamicEntity>()
                    .Select(de => de.TypedItem)
                    .ToList();
                // if (Debug) converted.ForEach(c => c.Debug = true);
                return l.Return(converted, "ent-list, now dyn");
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

        #region Parents / Children - ATM still dynamic

        public List<IDynamicEntity> Parents(IEntity entity, string type = null, string field = null)
            => entity.Parents(type, field).Select(e => SubDataFactory.SubDynEntityOrNull(e)).ToList();


        //public List<IEntity> ChildrenDecorated(IEntity entity, string field = null, string type = null)
        //    => /*Entity2Children(*/entity.Children(field, type) //, entity.EntityGuid, field);
        //        .Select((e, i) => EntityInBlockDecorator.Wrap(e, entity.EntityGuid, field, i) as IEntity)
        //        .ToList();


        public List<IDynamicEntity> Children(IEntity entity, string field = null, string type = null)
            => // ChildrenDecorated(entity, field, type)
                Entity2Children( entity.Children(field, type), entity.EntityGuid, field);
                //.Select((e, i) => EntityInBlockDecorator.Wrap(e, entity.EntityGuid, field, i))
                //.Select(e => SubDataFactory.SubDynEntityOrNull(e))
                //.ToList();

        private List<IDynamicEntity> Entity2Children(IEnumerable<IEntity> entities, Guid? parentGuid, string field = null)
            => entities
                .Select((e, i) => EntityInBlockDecorator.Wrap(e, parentGuid, field, i))
                .Select(e => SubDataFactory.SubDynEntityOrNull(e))
                .ToList();

        #endregion


    }
}
