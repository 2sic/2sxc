using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Plumbing
{
    /// <summary>
    /// Provide page scoped services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageScopedService<T> where T : class
    {
        public PageScopedService(PageScopeAccessor pageScopeAccessor)
        {
            _pageScopeAccessor = pageScopeAccessor;
        }
        private readonly PageScopeAccessor _pageScopeAccessor;

        public T Value => _pageScopeAccessor.ServiceProvider.Build<T>();
    }
}
