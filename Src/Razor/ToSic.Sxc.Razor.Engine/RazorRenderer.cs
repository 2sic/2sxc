using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ToSic.Sxc.Razor.Engine
{
    public class RazorRenderer : IRazorRenderer
    {
        #region Constructor and DI

        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorCompiler _razorCompiler;

        public RazorRenderer(ITempDataProvider tempDataProvider, IRazorCompiler razorCompiler)
        {
            _tempDataProvider = tempDataProvider;
            _razorCompiler = razorCompiler;
        }
        #endregion

        public async Task<string> RenderToStringAsync<TModel>(string partialName, TModel model, Action<RazorView> configure = null)
        {
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
            return output.ToString();
        }


        //private IView FindView(ActionContext actionContext, string partialName)
        //{
        //    var firstAttempt = _viewEngine.GetView(null, partialName, false);
        //    if (firstAttempt.Success)
        //        return firstAttempt.View;

        //    var secondAttempt = _viewEngine.FindView(actionContext, partialName, false);
        //    if (secondAttempt.Success)
        //        return secondAttempt.View;
        //    var searchedLocations = firstAttempt.SearchedLocations.Concat(secondAttempt.SearchedLocations);
        //    var errorMessage = string.Join(
        //        Environment.NewLine,
        //        new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations)); ;
        //    throw new InvalidOperationException(errorMessage);
        //}

        //private ActionContext NewActionContext()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
        //    return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        //}
    }
}