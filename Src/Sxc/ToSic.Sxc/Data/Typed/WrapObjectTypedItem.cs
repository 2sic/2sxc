using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Images;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Typed
{
    internal class WrapObjectTypedItem: WrapObjectTyped, ITypedItem
    {
        public WrapObjectTypedItem(PreWrapObject preWrap, DynamicWrapperFactory wrapperFactory) : base(preWrap, wrapperFactory)
        {
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

        int ITypedItem.Id => PreWrap.TryGet(nameof(ITypedItem.Id), noParamOrder: Protector, fallback: 0, required: false);

        Guid ITypedItem.Guid => PreWrap.TryGet(nameof(ITypedItem.Guid), noParamOrder: Protector, fallback: Guid.Empty, required: false);

        string ITypedItem.Title => _title.Get(() => 
            PreWrap.TryGet<string>(nameof(ITypedItem.Title), noParamOrder: Protector, fallback: null, required: false)
            ?? PreWrap.TryGet<string>("Name", noParamOrder: Protector, fallback: null, required: false));
        private readonly GetOnce<string> _title = new GetOnce<string>();

        #region Properties which return null or empty

        IEntity ICanBeEntity.Entity => null;
        IContentType ITypedItem.Type => null;
        ITypedItem ITypedItem.Child(string name, string noParamOrder, bool? required) => CreateItemFromProperty(name);

        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type, bool? required)
        {
            var blank = Enumerable.Empty<ITypedItem>();
            var (found, raw, _) = PreWrap.TryGet(field);
            if (!found || raw == null || raw.GetType().IsValueType) return blank;
            if (!(raw is IEnumerable re))
            {
                var rawWrapped = WrapperFactory.TypedItemFromObject(raw, PreWrap.Settings);
                return rawWrapped == null ? null : new[] { rawWrapped };
            }

            var list = re.Cast<object>()
                .Where(o => o != null && !o.GetType().IsValueType)
                .ToList();
            
            return list.Select(l => WrapperFactory.TypedItemFromObject(l, PreWrap.Settings));
        }

        /// <summary>
        /// The parents are "fake" so they behave just like children... but under the node "Parents".
        /// If "field" is specified, then it will assume another child-level under the node parents
        /// </summary>
        IEnumerable<ITypedItem> ITypedItem.Parents(string type, string noParamOrder, string field)
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

        IFolder ITypedItem.Folder(string name, string noParamOrder, bool? required) => null;

        IFile ITypedItem.File(string name, string noParamOrder, bool? required) => null;

        ITypedItem ITypedItem.Presentation => _presentation.Get(() => CreateItemFromProperty(nameof(ITypedItem.Presentation)));
        private readonly GetOnce<ITypedItem> _presentation = new GetOnce<ITypedItem>();

        private ITypedItem CreateItemFromProperty(string name)
        {
            var (found, raw, _) = PreWrap.TryGet(name);
            if (!found || raw == null || raw.GetType().IsValueType)
                return null;
            var first = raw is IEnumerable re ? re.Cast<object>().FirstOrDefault() : raw;
            if (first == null || first.GetType().IsValueType)
                return null;
            return WrapperFactory.TypedItemFromObject(first, PreWrap.Settings);
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

        IField ITypedItem.Field(string name, string noParamOrder, bool? required) => null;

        private NotSupportedException NotSupportedEx([CallerMemberName] string cName = default)
        {
            var ex = new NotSupportedException(
                $"You are accessing a virtual {nameof(ITypedItem)} which is based on an object, not an {nameof(IEntity)}. The method {cName}(...) doesn't work in this scenario.");
            return ex;
        }

        #endregion
    }
}
