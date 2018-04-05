using ToSic.SexyContent;

namespace ToSic.Sxc.Adam
{
    public interface IFile: ToSic.Eav.Apps.Assets.IFile
    {
        bool HasMetadata { get; }
        DynamicEntity Metadata { get; }
        string Name { get; }
        string Type { get; }
        string Url { get; }
    }
}