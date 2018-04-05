using ToSic.SexyContent;

namespace ToSic.Sxc.Adam
{
    public interface IFile
    {
        bool HasMetadata { get; }
        DynamicEntity Metadata { get; }
        string Name { get; }
        string Type { get; }
        string Url { get; }
    }
}