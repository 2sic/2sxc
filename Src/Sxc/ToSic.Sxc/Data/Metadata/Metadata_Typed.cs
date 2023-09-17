using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Images;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    internal partial class Metadata: ITypedItem
    {
        #region Keys

        [PrivateApi]
        public bool ContainsKey(string name) =>
            TypedHelpers.ContainsKey(name, Entity,
                (e, k) => e.Attributes.ContainsKey(k),
                (e, k) => e.Children(k)?.FirstOrDefault()
            );

        public bool IsEmpty(string name, string noParamOrder = Protector)//, bool? blankIs = default)
            => ItemHelper.IsEmpty(name, noParamOrder, default /*blankIs*/);

        public bool IsNotEmpty(string name, string noParamOrder = Protector)//, bool? blankIs = default)
            => ItemHelper.IsFilled(name, noParamOrder, default /*blankIs*/);

        [PrivateApi]
        public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
            => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

        #endregion

        #region ITyped

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
            => ItemHelper.Get(name, noParamOrder, required);

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
            => ItemHelper.Attribute(name, noParamOrder, fallback, required);

        [PrivateApi]
        dynamic ITypedItem.Dyn => this;


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml)
            => ItemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
            => ItemHelper.Url(name, noParamOrder, fallback, required);

        [PrivateApi]
        string ITyped.ToString() => "test / debug: " + ToString();

        #endregion

        #region ADAM

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name, string noParamOrder, bool? required) 
            => TypedItem.Folder(name, noParamOrder, required);

        IFile ITypedItem.File(string name, string noParamOrder, bool? required) 
            => TypedItem.File(name, noParamOrder, required);

        #endregion

        #region Basic Props like Id, Guid, Title, Type

        [PrivateApi]
        int IMetadata.EntityId => Cdf.CodeInfo.GetAndWarn(V16To18("IMetadata.EntityId", message: $"Use {nameof(ITypedItem.Id)} instead of {nameof(EntityId)}"), EntityId);

        [PrivateApi]
        int ITypedItem.Id => EntityId;

        [PrivateApi]
        Guid ITypedItem.Guid => EntityGuid;

        [PrivateApi]
        string ITypedItem.Title => EntityTitle;

        [PrivateApi]
        IContentType ITypedItem.Type => Entity?.Type;

        #endregion


        #region Relationship properties Presentation, Metadata, Child, Children, Parents

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Presentation => throw new NotSupportedException($"You can't access the {nameof(Presentation)} of Metadata");

        /// <inheritdoc />
        IMetadata ITypedItem.Metadata => throw new NotSupportedException($"You can't access the Metadata of Metadata in ITypedItem");

        [PrivateApi]
        public ITypedItem Parent() => throw new NotSupportedException($"You can't access the {nameof(Parent)} of Metadata");

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string noParamOrder, string type, string field)
        {
            return TypedItem.Parents(noParamOrder: noParamOrder, type: type, field: field);
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type, bool? required)
        {
            return TypedItem.Children(field: field, noParamOrder: noParamOrder, type: type, required: required);
            //Protect(noParamOrder, $"{nameof(type)}, {nameof(required)}");

            //if (IsErrStrict(this, field, required, GetHelper.StrictGet))
            //    throw ErrStrict(field);

            //var dynChildren = Children(field, type);
            //var list = Cdf.AsItems(dynChildren, noParamOrder).ToList();
            //if (list.Any()) return list;

            //// Generate a marker/placeholder to remember what field this is etc.
            //var fakeEntity = GetHelper.Cdf.PlaceHolderInBlock(Entity.AppId, Entity, field);
            //return new ListTypedItems(new List<ITypedItem>(), fakeEntity);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string name, string noParamOrder, bool? required)
        {
            return TypedItem.Child(name, noParamOrder, required);
            //Protect(noParamOrder, nameof(required));
            //return IsErrStrict(this, name, required, GetHelper.StrictGet)
            //    ? throw ErrStrict(name)
            //    : (this as ITypedItem).Children(name).FirstOrDefault();
        }

        #endregion

        #region Fields, Html, Picture

        [PrivateApi]
        IField ITypedItem.Field(string name, string noParamOrder, bool? required) => Cdf.Field(this, name, GetHelper.PropsRequired, noParamOrder, required);

        IHtmlTag ITypedItem.Html(
            string name,
            string noParamOrder,
            object container,
            bool? toolbar,
            object imageSettings,
            bool? required,
            bool debug
        ) => TypedItemHelpers.Html(Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
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
            object toolbar,
            object recipe
        ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, settings: settings,
            factor: factor, width: width, imgAlt: imgAlt,
            imgAltFallback: imgAltFallback, imgClass: imgClass, toolbar: toolbar, recipe: recipe);

        #endregion

    }
}
