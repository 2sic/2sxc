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

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name) => _adamCache.Get(name, () => _Services.AsC.Folder(Entity, name)); // .AdamManager.Folder(Entity, name));
        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        IFile ITypedItem.File(string name)
        {
            ThrowIfKitNotAvailable();
            var file = _Services.Kit.Adam.File(Field(name));
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

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string type, string noParamOrder, string field)
        {
            Protect(noParamOrder, $"{nameof(field)}");
            return _Services.AsC.AsItems(Parents(type, field));
        }

        /// <inheritdoc />
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type)
        {
            Protect(noParamOrder, $"{nameof(type)}");
            var dynChildren = Children(field, type);
            var list = _Services.AsC.AsItems(dynChildren).ToList();
            if (list.Any()) return list;

            // Generate a marker/placeholder to remember what field this is etc.
            var fakeEntity = PlaceHolder(Entity.AppId, Entity, field);
            return new ListTypedItems(list, fakeEntity);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string field) => (this as ITypedItem).Children(field).FirstOrDefault();
    }
}
