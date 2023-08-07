using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: ITypedItem
    {
        [PrivateApi]
        bool ITyped.ContainsKey(string name)
        {
            var parts = PropertyStack.SplitPathIntoParts(name);
            if (!parts.Any()) return false;

            var parentEntity = Entity;
            var max = parts.Length - 1;
            for (var i = 0; i < parts.Length; i++)
            {
                var key = parts[i];
                var has = parentEntity.Attributes.ContainsKey(key);
                if (i == max || !has) return has;

                // has = true, and we have more nodes, so we must check the children
                var children = parentEntity.Children(key);
                if (!children.Any()) return false;
                parentEntity = children[0];
            }

            return false;
        }

        [PrivateApi]
        protected bool IsErrStrict(string name, bool? strict, bool strictGetDefault)
            => !(this as ITyped).ContainsKey(name) && (strict ?? strictGetDefault);


        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name, string noParamOrder, bool? strict)
        {
            Protect(noParamOrder, nameof(strict));
            return IsErrStrict(name, strict, StrictGet)
                ? throw ErrStrict(name)
                : _adamCache.Get(name, () => _Services.AsC.Folder(Entity, name));
        }

        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        IFile ITypedItem.File(string name, string noParamOrder, bool? strict)
        {
            Protect(noParamOrder, nameof(strict));
            var typedThis = this as ITypedItem;
            // Case 1: The field contains a direct reference to a file
            var file = GetServiceKitOrThrow().Adam.File(typedThis.Field(name, strict: strict));
            // Case 2: No direct reference, just get the first file in the folder of this field
            return file ?? typedThis.Folder(name).Files.FirstOrDefault();
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
            Protect(noParamOrder, nameof(field));
            return _Services.AsC.AsItems(Parents(type, field), noParamOrder);
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type, bool? strict)
        {
            Protect(noParamOrder, $"{nameof(type)}, {nameof(strict)}");

            if (IsErrStrict(field, strict, StrictGet))
                throw ErrStrict(field);

            var dynChildren = Children(field, type);
            var list = _Services.AsC.AsItems(dynChildren, noParamOrder).ToList();
            if (list.Any()) return list;

            // Generate a marker/placeholder to remember what field this is etc.
            var fakeEntity = PlaceHolder(Entity.AppId, Entity, field);
            return new ListTypedItems(new List<ITypedItem>(), fakeEntity);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string name, string noParamOrder, bool? strict)
        {
            Protect(noParamOrder, nameof(strict));
            return IsErrStrict(name, strict, StrictGet)
                ? throw ErrStrict(name)
                : (this as ITypedItem).Children(name).FirstOrDefault();
        }
    }
}
