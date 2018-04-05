using System.Collections.Generic;
using System.Linq;
using ToSic.SexyContent.Adam;
using ToSic.Sxc.Adam;

namespace ToSic.SexyContent.Obsolete.Adam
{
    public class AdamFolder : IFolder
    {
        private readonly IFolder _original;
        public AdamFolder(IFolder original)
        {
            _original = original;
        }

        public IEnumerable<AdamFile> Files => _original.Files.Select(f => new AdamFile(f));
        public IEnumerable<AdamFolder> Folders => _original.Folders.Select(f => new AdamFolder(f));


        IEnumerable<Folder> IFolder.Folders => _original.Folders;

        public bool HasMetadata => _original.HasMetadata;

        public DynamicEntity Metadata => _original.Metadata;

        public string Type => _original.Type;

        public string Url => _original.Url;

        IEnumerable<File> IFolder.Files => _original.Files;

    }
}

