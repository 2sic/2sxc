using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem
    {
        /// <inheritdoc />
        public IFolderTyped Folder(string name) => _adamCache.Get(name, () => _typedHelpers.AdamManager.Folder(Entity, name) as IFolderTyped);
        private readonly GetOnceNamed<IFolderTyped> _adamCache = new GetOnceNamed<IFolderTyped>();

        // TODO: MUST handle all edge cases first
        // Eg. Hyperlink field should return the file which was selected, not any first file in the folder
        //public IFile File(string name) => Folder(name).Files.FirstOrDefault();

        ///// <inheritdoc />
        //public IEnumerable<IFileTyped> Files(string name) => Folder(name)?.Files ?? new List<IFileTyped>();

        ///// <inheritdoc />
        //public IEnumerable<IFolderTyped> Folders(string name) => Folder(name)?.Folders ?? new List<IFolderTyped>();
    }
}
