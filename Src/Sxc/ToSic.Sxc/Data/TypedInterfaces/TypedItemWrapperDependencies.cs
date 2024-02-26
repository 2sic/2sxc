using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data;

/// <summary>
/// Helper object to ensure that typed item wrappers have everything to generate more typed items
/// </summary>
internal class TypedItemWrapperDependencies(ServiceKit16 addKit)
{
    public ServiceKit16 AddKit { get; } = addKit;
}