using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace ToSic.Sxc.Razor
{
    public interface IRazorRenderer
    {
        Task<string> RenderToStringAsync<TModel>(string partialName, TModel model, Action<RazorView> configure = null);
    }
}