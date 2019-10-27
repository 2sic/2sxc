using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Adam
{
    public interface IFile : Eav.Apps.Assets.IFile
    {
        bool HasMetadata { get; }
        IDynamicEntity Metadata { get; }
        string Name { get; }
        string Type { get; }
        string Url { get; }
    }
}
