using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Code.IDynamicCodeRoot16AsExtensions;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: ITypedItem
    {

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name) => _adamCache.Get(name, () => _Services.AdamManager.Folder(Entity, name));
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
            return AsTypedList(Parents(type, field), _Services, 3, _Services.LogOrNull);
        }

        /// <inheritdoc />
#pragma warning disable CS1066
        [PrivateApi]
        IEnumerable<ITypedItem> ITypedItem.Children(string field = default, string noParamOrder = Protector, string type = default)
#pragma warning restore CS1066
        {
            Protect(noParamOrder, $"{nameof(type)}");
            return AsTypedList(Children(field, type), _Services, 3, _Services.LogOrNull);
        }

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Child(string field) => (this as ITypedItem).Children(field).FirstOrDefault();
    }
}
