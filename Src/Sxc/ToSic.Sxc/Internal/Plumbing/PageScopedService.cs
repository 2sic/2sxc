using ToSic.Lib.DI;

namespace ToSic.Sxc.Internal.Plumbing;

/// <summary>
/// Provide page scoped services
/// </summary>
/// <typeparam name="T"></typeparam>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageScopedService<T>(PageScopeAccessor pageScopeAccessor) where T : class
{
    public T Value => pageScopeAccessor.ServiceProvider.Build<T>();
}