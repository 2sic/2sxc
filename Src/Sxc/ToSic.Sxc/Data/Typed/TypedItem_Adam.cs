using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Data
{
    public partial class TypedItem
    {
        /// <inheritdoc />
        public IFolder Folder(string name) => _adamCache.Get(name, () => _Services.AdamManager.Folder(Entity, name) as IFolder);
        private readonly GetOnceNamed<IFolder> _adamCache = new GetOnceNamed<IFolder>();

        // TODO: MUST handle all edge cases first
        // Eg. Hyperlink field should return the file which was selected, not any first file in the folder
        //public IFile File(string name) => Folder(name).Files.FirstOrDefault();
    }
}
