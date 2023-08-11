using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Images;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: ITypedItem
    {
        #region Keys

        [PrivateApi]
        bool ITyped.ContainsKey(string name) =>
            TypedHelpers.ContainsKey(name, Entity,
                (e, k) => e.Attributes.ContainsKey(k),
                (e, k) => e.Children(k)?.FirstOrDefault()
            );

        [PrivateApi]
        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only) 
            => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

        #endregion

        #region ADAM

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, Helper.StrictGet)
                ? throw ErrStrict(name)
                : _adamCache.Get(name, () => _Cdf.Folder(Entity, name, (this as ITypedItem).Field(name, required: false)));
        }
        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        IFile ITypedItem.File(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            var typedThis = this as ITypedItem;
            // Case 1: The field contains a direct reference to a file
            var field = typedThis.Field(name, required: required);
            var file = _Cdf.GetServiceKitOrThrow().Adam.File(field);
            // Case 2: No direct reference, just get the first file in the folder of this field
            return file ?? typedThis.Folder(name).Files.FirstOrDefault();
        }

        #endregion

        [PrivateApi]
        int ITypedItem.Id => EntityId;

        [PrivateApi]
        Guid ITypedItem.Guid => EntityGuid;

        [PrivateApi]
        string ITypedItem.Title => EntityTitle;

        [PrivateApi]
        IContentType ITypedItem.Type => Entity?.Type;

        #region Relationship properties Presentation, Metadata, Child, Children, Parents

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Presentation => PreWrap.Presentation;

        /// <inheritdoc />
        IMetadata ITypedItem.Metadata => PreWrap.Metadata;

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string noParamOrder, string type, string field)
        {
            Protect(noParamOrder, nameof(field), message: 
                $" ***IMPORTANT***: The typed '.Parents(...)' method was changed to also make the parameter '{nameof(type)}' required. " +
                "So if you had '.Parents(something)' then change it to '.Parents(type: something)'");
            return _Cdf.AsItems(Parents(type, field), noParamOrder);
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type, bool? required)
        {
            Protect(noParamOrder, $"{nameof(type)}, {nameof(required)}");

            if (IsErrStrict(this, field, required, Helper.StrictGet))
                throw ErrStrict(field);

            var dynChildren = Children(field, type);
            var list = _Cdf.AsItems(dynChildren, noParamOrder).ToList();
            if (list.Any()) return list;

            // Generate a marker/placeholder to remember what field this is etc.
            var fakeEntity = Helper.PlaceHolder(Entity.AppId, Entity, field);
            return new ListTypedItems(new List<ITypedItem>(), fakeEntity);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, Helper.StrictGet)
                ? throw ErrStrict(name)
                : (this as ITypedItem).Children(name).FirstOrDefault();
        }

        #endregion

        #region Fields, Html, Picture

        [PrivateApi]
        IField ITypedItem.Field(string name, string noParamOrder, bool? required) => ItemHelper.Field(this, name, noParamOrder, required);

        IHtmlTag ITypedItem.Html(
            string name,
            string noParamOrder,
            object container,
            bool? toolbar,
            object imageSettings,
            bool? required,
            bool debug
        ) => TypedItemHelpers.Html(_Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
            toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug);

        /// <inheritdoc/>
        IResponsivePicture ITypedItem.Picture(
            string name,
            string noParamOrder,
            object settings,
            object factor,
            object width,
            string imgAlt,
            string imgAltFallback,
            string imgClass,
            object recipe
        ) => TypedItemHelpers.Picture(cdf: _Cdf, item: this, name: name, noParamOrder: noParamOrder, settings: settings,
            factor: factor, width: width, imgAlt: imgAlt,
            imgAltFallback: imgAltFallback, imgClass: imgClass, recipe: recipe);

        #endregion
    }
}
