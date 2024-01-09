using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Razor.Internal;

namespace ToSic.Sxc.Razor
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [PrivateApi("used to be marked as internal, but it doesn't make sense to show in docs")]
    [EngineDefinition(Name = "Razor")]
    internal class NetCoreRazorEngine : EngineBase, IRazorEngine
    {
        private readonly LazySvc<CodeErrorHelpService> _errorHelp;
        private readonly LazySvc<CodeRootFactory> _codeRootFactory;
        private readonly LazySvc<IRenderingHelper> _renderingHelper;
        public IRazorRenderer RazorRenderer { get; }

        #region Constructor / DI

        public NetCoreRazorEngine(MyServices services, IRazorRenderer razorRenderer, LazySvc<CodeRootFactory> codeRootFactory, LazySvc<CodeErrorHelpService> errorHelp, LazySvc<IRenderingHelper> renderingHelper) : base(services)
        {
            ConnectServices(
                _codeRootFactory = codeRootFactory,
                RazorRenderer = razorRenderer,
                _errorHelp = errorHelp,
                _renderingHelper = renderingHelper
            );
        }

        #endregion

        /// <inheritdoc/>
        protected override (string, List<Exception>) RenderImplementation(object data)
        {
            var l = Log.Fn<(string, List<Exception>)>();
            var task = RenderTask();
            try
            {
                task.Wait();
                var result = task.Result;

                if (result.Exception == null) return l.ReturnAsOk((result.TextWriter.ToString(), null));

                var errorMessage = _renderingHelper.Value.Init(Block).DesignErrorMessage(new () { result.Exception }, true);
                return l.Return((errorMessage, new() { result.Exception }));
            }
            catch (Exception ex)
            {
                var myEx = task.Exception?.InnerException ?? ex;
                return l.Return((myEx.ToString(), new() { myEx }));
            }
        }

        [PrivateApi]
        private async Task<(TextWriter TextWriter, Exception Exception)> RenderTask()
        {
            Log.A("will render into TextWriter");
            RazorView page = null;
            try
            {
                if (string.IsNullOrEmpty(TemplatePath)) return (null, null);

                var result = await RazorRenderer.RenderToStringAsync(TemplatePath, new object(),
                    rzv =>
                    {
                        page = rzv; // keep for better errors
                        if (rzv.RazorPage is not IRazor asSxc) return;

                        var dynCode = _codeRootFactory.Value
                            .BuildCodeRoot(asSxc, Block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel12);

                        asSxc.ConnectToRoot(dynCode);
                        // Note: Don't set the purpose here any more, it's a deprecated feature in 12+
                    }, App);
                var writer = new StringWriter();
                await writer.WriteAsync(result);
                return (writer, null);
            }
            catch (Exception maybeIEntityCast)
            {
                return (null, _errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
            }

            // WIP https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs#L397-L404
            // maybe also https://stackoverflow.com/questions/48206993/how-to-load-asp-net-core-razor-view-dynamically-at-runtime
            // later also check loading more DLLs on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime

        }
    }
}