using ToSic.Sxc.Adam;

namespace ToSic.SexyContent.Obsolete.Adam
{
    public class FolderOfFieldCompatibility : AdamFolder, IFolderOfField
    {
        private readonly IFolderOfField _original;
        public FolderOfFieldCompatibility(IFolderOfField original) : base(original)
        {
            _original = original;
        }

        public bool Exists => _original.Exists;
    }
}

