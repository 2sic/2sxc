using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace ToSic.Sxc.Razor
{
    public interface IRazorRenderer
    {
        Task<string> RenderToStringAsync<TModel>(string templatePath, TModel model, Action<RazorView> configure = null, string appCodeFullPath = null, string templateFullPath = null);
    }
}