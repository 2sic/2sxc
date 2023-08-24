using System;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Apps
{
    public interface IAppConfiguration: IEntityBasedType
    {
        Version Version { get; }
        string Name { get; }
        string Description { get; }
        string Folder { get; }
        bool EnableRazor { get; }
        bool EnableToken { get; }
        bool IsHidden { get; }
        bool EnableAjax { get; }
        Guid OriginalId { get; }
        Version RequiredSxc { get; }
        Version RequiredDnn { get; }
        Version RequiredOqtane { get; }
    }
}