using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: ITypedItem
    {

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name)
        {
            if (StrictGet && !Entity.Attributes.ContainsKey(name))
                throw new ArgumentException(ErrStrict(name));

            return _adamCache.Get(name, () => _Services.AsC.Folder(Entity, name));
        }

        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        IFile ITypedItem.File(string name)
        {
            var file = GetServiceKitOrThrow().Adam.File(Field(name));
            return file ?? (this as ITypedItem).Folder(name).Files.FirstOrDefault();
        }

        [PrivateApi]
        int ITypedItem.Id => EntityId;

        [PrivateApi]
        Guid ITypedItem.Guid => EntityGuid;

        [PrivateApi]
        string ITypedItem.Title => EntityTitle;

        [PrivateApi]
        IContentType ITypedItem.Type => Entity?.Type;

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Presentation => Presentation;

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string type, string noParamOrder, string field)
        {
            Protect(noParamOrder, $"{nameof(field)}");
            return _Services.AsC.AsItems(Parents(type, field), noParamOrder);
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type)
        {
            Protect(noParamOrder, $"{nameof(type)}");

            if (StrictGet && !Entity.Attributes.ContainsKey(field))
                throw new ArgumentException(ErrStrict(field));

            var dynChildren = Children(field, type);
            var list = _Services.AsC.AsItems(dynChildren, noParamOrder).ToList();
            if (list.Any()) return list;

            // Generate a marker/placeholder to remember what field this is etc.
            var fakeEntity = PlaceHolder(Entity.AppId, Entity, field);
            return new ListTypedItems(new List<ITypedItem>(), fakeEntity);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string name, string noParamOrder)
        {
            Protect(noParamOrder);
            if (StrictGet && !Entity.Attributes.ContainsKey(name))
                throw new ArgumentException(ErrStrict(name));
            return (this as ITypedItem).Children(name).FirstOrDefault();
        }
    }
}
