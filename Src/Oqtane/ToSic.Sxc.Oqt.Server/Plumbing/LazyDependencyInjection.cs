using System;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Oqt.Server.Plumbing
{
    public class LazyDependencyInjection<T> : Lazy<T>
    {
        public LazyDependencyInjection(IServiceProvider sp) : base(sp.GetRequiredService<T>())
        {
        }

    }
}
