using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

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
        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only) 
            => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, StrictGet)
                ? throw ErrStrict(name)
                : _adamCache.Get(name, () => _Cdf.Folder(Entity, name));
        }

        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        IFile ITypedItem.File(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            var typedThis = this as ITypedItem;
            // Case 1: The field contains a direct reference to a file
            var field = typedThis.Field(name, required: required);
            var file = GetServiceKitOrThrow().Adam.File(field);
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

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string type, string noParamOrder, string field)
        {
            Protect(noParamOrder, nameof(field));
            return _Cdf.AsItems(Parents(type, field), noParamOrder);
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type, bool? required)
        {
            Protect(noParamOrder, $"{nameof(type)}, {nameof(required)}");

            if (IsErrStrict(this, field, required, StrictGet))
                throw ErrStrict(field);

            var dynChildren = Children(field, type);
            var list = _Cdf.AsItems(dynChildren, noParamOrder).ToList();
            if (list.Any()) return list;

            // Generate a marker/placeholder to remember what field this is etc.
            var fakeEntity = PlaceHolder(Entity.AppId, Entity, field);
            return new ListTypedItems(new List<ITypedItem>(), fakeEntity);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return IsErrStrict(this, name, required, StrictGet)
                ? throw ErrStrict(name)
                : (this as ITypedItem).Children(name).FirstOrDefault();
        }
    }
}
