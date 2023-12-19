using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Threading.Tasks;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Razor
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IRazorCompiler
    {
        Task<(IView view, ActionContext context)> CompileView(string templatePath, Action<RazorView> configure = null, IApp app = null);
    }
}
