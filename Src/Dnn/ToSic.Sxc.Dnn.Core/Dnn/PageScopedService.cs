using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// Provide page scoped services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageScopedService<T> where T : class
    {
        private readonly PageScopeAccessor _pageScopeAccessor;

        public PageScopedService(PageScopeAccessor pageScopeAccessor)
        {
            _pageScopeAccessor = pageScopeAccessor;
        }

        public T Value => _pageScopeAccessor.Scope.ServiceProvider.GetRequiredService<T>();
    }
}
