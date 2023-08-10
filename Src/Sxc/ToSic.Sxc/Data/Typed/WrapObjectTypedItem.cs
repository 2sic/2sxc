using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Images;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data.Typed
{
    internal class WrapObjectTypedItem: WrapObjectTyped, ITypedItem
    {
        private readonly ILazyLike<CodeDataFactory> _cdf;

        public WrapObjectTypedItem(PreWrapObject preWrap, CodeDataWrapper wrapper, ILazyLike<CodeDataFactory> cdf) : base(preWrap, wrapper)
        {
            _cdf = cdf;
        }


        bool ITypedItem.IsDemoItem => PreWrap.TryGet(nameof(ITypedItem.IsDemoItem), noParamOrder: Protector, fallback: false, required: false);

        IHtmlTag ITypedItem.Html(string name, string noParamOrder, object container, bool? toolbar,
            object imageSettings, bool? required, bool debug
        ) => TypedItemHelpers.Html(_cdf.Value, this, name: name, noParamOrder: noParamOrder, container: container,
            toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug);

        IResponsivePicture ITypedItem.Picture(string name, string noParamOrder, object settings,
            object factor, object width, string imgAlt, string imgAltFallback,
            string imgClass, object recipe
        ) => TypedItemHelpers.Picture(cdf: _cdf.Value, item: this, name: name, noParamOrder: noParamOrder,
            settings: settings, factor: factor, width: width, imgAlt: imgAlt,
            imgAltFallback: imgAltFallback, imgClass: imgClass, recipe: recipe);


        public int Id => PreWrap.TryGet(nameof(Id), noParamOrder: Protector, fallback: 0, required: false);

        public Guid Guid => PreWrap.TryGet(nameof(Guid), noParamOrder: Protector, fallback: Guid.Empty, required: false);

        public string Title => _title.Get(() => PreWrap.TryGet<string>(nameof(ITypedItem.Title), noParamOrder: Protector, fallback: null, required: false));
        private readonly GetOnce<string> _title = new GetOnce<string>();

        #region Properties which return null or empty

        public IEntity Entity => null;
        public IContentType Type => null;

        #region Relationships - Child, Children, Parents, Presentation

        public ITypedItem Child(string name, string noParamOrder, bool? required) => CreateItemFromProperty(name);

        public IEnumerable<ITypedItem> Children(string field, string noParamOrder, string type, bool? required)
        {
            var blank = Enumerable.Empty<ITypedItem>();
            var (found, raw, _) = PreWrap.TryGet(field);
            if (!found || raw == null || raw.GetType().IsValueType) return blank;
            if (!(raw is IEnumerable re))
            {
                var rawWrapped = Wrapper.TypedItemFromObject(raw, PreWrap.Settings);
                return rawWrapped == null ? null : new[] { rawWrapped };
            }

            var list = re.Cast<object>()
                .Where(o => o != null && !o.GetType().IsValueType)
                .ToList();
            
            return list.Select(l => Wrapper.TypedItemFromObject(l, PreWrap.Settings));
        }

        /// <summary>
        /// The parents are "fake" so they behave just like children... but under the node "Parents".
        /// If "field" is specified, then it will assume another child-level under the node parents
        /// </summary>
        public IEnumerable<ITypedItem> Parents(string noParamOrder, string type, string field)
        {
            var blank = Enumerable.Empty<ITypedItem>();
            var typed = this as ITypedItem;
            var items = typed.Children(nameof(ITypedItem.Parents), noParamOrder)?.ToList();

            if (items == null || !items.Any() || !type.HasValue() && !field.HasValue())
                return items ?? blank;

            if (type.HasValue())
                items = items.Where(i => i.String(nameof(ITypedItem.Type), required: false).EqualsInsensitive(type)).ToList();

            if (field.HasValue())
                items = items.Where(i => i.String("Field", required: false).EqualsInsensitive(field)).ToList();
            return items;
        }

        public ITypedItem Presentation => _presentation.Get(() => CreateItemFromProperty(nameof(ITypedItem.Presentation)));
        private readonly GetOnce<ITypedItem> _presentation = new GetOnce<ITypedItem>();

        private ITypedItem CreateItemFromProperty(string name)
        {
            var (found, raw, _) = PreWrap.TryGet(name);
            if (!found || raw == null || raw.GetType().IsValueType)
                return null;
            var first = raw is IEnumerable re ? re.Cast<object>().FirstOrDefault() : raw;
            if (first == null || first.GetType().IsValueType)
                return null;
            return Wrapper.TypedItemFromObject(first, PreWrap.Settings);
        }

        #endregion


        public IFolder Folder(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, PreWrap.Settings.GetStrict)
                ? throw ErrStrict(name)
                : _cdf.Value.AdamManager.Folder(Guid, name, Field(name, noParamOrder, required));
        }

        public IFile File(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            if (IsErrStrict(this, name, required, PreWrap.Settings.GetStrict))
                throw ErrStrict(name);
            var typed = this as ITypedItem;
            // Check if it's a direct string, or an object with a sub-property with a Value
            var idString = typed.String(name) ?? typed.Child(name)?.String("Value");

            // TODO: SEE if we can also provide optional metadata

            var fileId = AdamManager.CheckIdStringForId(idString);
            return fileId == null ? null : _cdf.Value.AdamManager.File(fileId.Value);
        }

        #endregion

        #region Not Supported Properties such as Entity, Type, Child, Folder, Presentation, Metadata

        IMetadata ITypedItem.Metadata => _metadata ?? (_metadata = BuildMetadata(PreWrap.TryGet(nameof(Metadata)).Raw));
        private Metadata _metadata;

        private Metadata BuildMetadata(object raw)
        {
            var objList = raw != null
                ? raw is IEnumerable rawEnum
                    ? rawEnum.Cast<object>().ToList()
                    : new List<object> { raw }
                : new List<object>();

            var df = _cdf.Value.Services.DataFactory.New(
                options: new DataFactoryOptions(appId: _cdf.Value.BlockOrNull?.AppId, autoId: false));
            var mdEntities = objList
                .Where(o => o != null)
                .Select(o =>
                {
                    var values = o.ToDicInvariantInsensitive();
                    // Note: id/guid don't really work, but it's never used in metadata context
                    //var id = values.TryGetValue(nameof(Id), out var maybeId) ? maybeId.ConvertOrFallback(0) : 0;
                    //var guid = values.TryGetValue(nameof(Guid), out var maybeGuid) ? maybeGuid.ConvertOrFallback(Guid.Empty) : Guid.Empty;
                    return df.Create(values); //, id: id, guid: guid);
                })
                .ToList();

            var mdOf = new MetadataOf<int>(0, 0, "virtual", mdEntities);
            var metadata = new Metadata(mdOf, parentOrNull: null, _cdf.Value);
            ;
            return metadata;
        }


        public IField Field(string name, string noParamOrder, bool? required) 
            => new Field(this, name, _cdf.Value);

        /// <summary>
        /// Override the URL, to also support checks for "file:72"
        /// </summary>
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback, required: required);
            if (url == null) return null;

            // ReSharper disable once ConvertTypeCheckPatternToNullCheck
            if (ValueConverterBase.CouldBeReference(url))
                url = _cdf.Value.Services.ValueConverterOrNull?.ToValue(url, Guid.Empty) ?? url;

            return Tags.SafeUrl(url).ToString();
        }
        #endregion
    }
}
