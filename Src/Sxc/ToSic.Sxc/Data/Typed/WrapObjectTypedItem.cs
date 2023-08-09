using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Images;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

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
            object imageSettings, bool? required, bool debug)
        {
            throw new NotImplementedException();
        }

        IResponsivePicture ITypedItem.Picture(string name, string noParamOrder, object settings,
            object factor, object width, string imgAlt, string imgAltFallback,
            string imgClass, object recipe)
        {
            throw new NotImplementedException();
        }

        public int Id => PreWrap.TryGet(nameof(Id), noParamOrder: Protector, fallback: 0, required: false);

        public Guid Guid => PreWrap.TryGet(nameof(Guid), noParamOrder: Protector, fallback: Guid.Empty, required: false);

        public string Title => _title.Get(() => 
            PreWrap.TryGet<string>(nameof(ITypedItem.Title), noParamOrder: Protector, fallback: null, required: false)
            ?? PreWrap.TryGet<string>("Name", noParamOrder: Protector, fallback: null, required: false));
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
        public IEnumerable<ITypedItem> Parents(string type, string noParamOrder, string field)
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
                : _cdf.Value.AdamManager.Folder(Guid, name);
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

        IMetadata ITypedItem.Metadata
        {
            get
            {
                // TODO: maybe consider creating virtual metadata based on the data provided?
                throw NotSupportedEx();
            }
        }

        public IField Field(string name, string noParamOrder, bool? required) 
            => new Field(this, name, _cdf.Value);

        private NotSupportedException NotSupportedEx([CallerMemberName] string cName = default)
        {
            var ex = new NotSupportedException(
                $"You are accessing a virtual {nameof(ITypedItem)} which is based on an object, not an {nameof(IEntity)}. The method {cName}(...) doesn't work in this scenario.");
            return ex;
        }

        #endregion
    }
}
