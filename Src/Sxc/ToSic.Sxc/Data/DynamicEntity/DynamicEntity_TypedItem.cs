using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using static ToSic.Sxc.Code.IDynamicCodeRoot16AsExtensions;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: ITypedItem
    {

        /// <inheritdoc />
        [PrivateApi]
        IFolder ITypedItem.Folder(string name) => _adamCache.Get(name, () => _Services.AdamManager.Folder(Entity, name) as IFolder);
        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        // TODO: MUST handle all edge cases first
        // Eg. Hyperlink field should return the file which was selected, not any first file in the folder
        //public IFile File(string name) => Folder(name).Files.FirstOrDefault();


        [PrivateApi]
        dynamic ITypedItem.Dyn => this;

        /// <inheritdoc />
        [PrivateApi]
        ITypedItem ITypedItem.Presentation => Presentation;

        /// <inheritdoc />
#pragma warning disable CS1066
        IEnumerable<ITypedItem> ITypedItem.Parents(string type = default, string noParamOrder = Parameters.Protector, string field = default)
#pragma warning restore CS1066
        {
            Parameters.Protect(noParamOrder, $"{nameof(field)}");
            return AsTypedList(Parents(type, field), _Services, 3, _Services.LogOrNull);
        }

        /// <inheritdoc />
#pragma warning disable CS1066
        IEnumerable<ITypedItem> ITypedItem.Children(string field = default, string noParamOrder = Parameters.Protector, string type = default)
#pragma warning restore CS1066
        {
            Parameters.Protect(noParamOrder, $"{nameof(type)}");
            return AsTypedList(Children(field, type), _Services, 3, _Services.LogOrNull);
        }

        /// <inheritdoc />
        ITypedItem ITypedItem.Child(string field) => (this as ITypedItem).Children(field).FirstOrDefault();
    }
}
