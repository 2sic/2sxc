using System.Collections.Generic;
using ToSic.SexyContent;

namespace ToSic.Sxc.Adam
{
    public interface IFolder
    {
        IEnumerable<File> Files { get; }
        IEnumerable<Folder> Folders { get; }
        bool HasMetadata { get; }
        DynamicEntity Metadata { get; }
        string Type { get; }
        string Url { get; }
    }
}