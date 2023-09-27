using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Threading.Tasks;

namespace ToSic.Sxc.Razor
{
    public interface IRazorCompiler
    {
        Task<(IView view, ActionContext context)> CompileView(string templatePath, Action<RazorView> configure = null,
            string appCodeFullPath = null, string templateFullPath = null);
    }
}
