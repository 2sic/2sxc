using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Razor
{
    public class RazorRenderer : ServiceBase, IRazorRenderer
    {
        #region Constructor and DI

        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorCompiler _razorCompiler;

        public RazorRenderer(ITempDataProvider tempDataProvider, IRazorCompiler razorCompiler) : base($"{Constants.SxcLogName}.RzrRdr")
        {
            ConnectServices(
                _tempDataProvider = tempDataProvider,
                _razorCompiler = razorCompiler
            );
        }
        #endregion

        public async Task<string> RenderToStringAsync<TModel>(string partialName, TModel model, Action<RazorView> configure = null)
        {
            var l = Log.Fn<string>($"partialName:{partialName}");
            //var actionContext = _actionContextAccessor.ActionContext;
            //var partial = FindView(actionContext, partialName);
            //// do callback to configure the object we received
            //if (partial is RazorView rzv) configure?.Invoke(rzv);

            var (view, actionContext) = _razorCompiler.CompileView(partialName, configure);

            // Prepare to render
            await using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions()
            );
            await view.RenderAsync(viewContext);
            return l.ReturnAsOk(output.ToString());
        }
    }
}