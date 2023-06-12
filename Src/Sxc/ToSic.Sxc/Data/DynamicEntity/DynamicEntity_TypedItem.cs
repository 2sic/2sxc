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
#pragma warning disable CS1066
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Parents(string type = default, string noParamOrder = Protector, string field = default)
#pragma warning restore CS1066
        {
            Protect(noParamOrder, $"{nameof(field)}");
            return _Services.AsC.AsTypedList(Parents(type, field));
        }

        /// <inheritdoc />
#pragma warning disable CS1066
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field = default, string noParamOrder = Protector, string type = default)
#pragma warning restore CS1066
        {
            Protect(noParamOrder, $"{nameof(type)}");
            var dynChildren = Children(field, type);
            var list = _Services.AsC.AsTypedList(dynChildren).ToList();
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
