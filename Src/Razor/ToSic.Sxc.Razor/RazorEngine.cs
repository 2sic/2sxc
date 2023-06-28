using System;
using System.IO;
using System.Threading.Tasks;
using Custom.Hybrid;
using Microsoft.AspNetCore.Mvc.Razor;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [PrivateApi("used to be marked as internal, but it doesn't make sense to show in docs")]
    [EngineDefinition(Name = "Razor")]

    public partial class RazorEngine : EngineBase, IRazorEngine
    {
        private readonly LazySvc<CodeErrorHelpService> _errorHelp;
        private readonly LazySvc<DynamicCodeRoot> _dynCodeRootLazy;
        public IRazorRenderer RazorRenderer { get; }

        #region Constructor / DI

        public RazorEngine(MyServices services, IRazorRenderer razorRenderer, LazySvc<DynamicCodeRoot> dynCodeRootLazy, LazySvc<CodeErrorHelpService> errorHelp) : base(services)
        {
            ConnectServices(
                _dynCodeRootLazy = dynCodeRootLazy,
                RazorRenderer = razorRenderer,
                _errorHelp = errorHelp
            );
        }
        
        #endregion

        /// <inheritdoc/>
        protected override (string, Exception) RenderTemplate(object data)
        {
            var l = Log.Fn<(string, Exception)>();
            var task = RenderTask();
            task.Wait();
            return l.ReturnAsOk((task.Result.ToString(), null));
        }

        [PrivateApi]
        public async Task<TextWriter> RenderTask()
        {
            Log.A("will render into TextWriter");
            RazorView page = null;
            try
            {
                if (string.IsNullOrEmpty(TemplatePath)) return null;
                var dynCode = _dynCodeRootLazy.Value.InitDynCodeRoot(Block, Log, Constants.CompatibilityLevel12);

                var result = await RazorRenderer.RenderToStringAsync(TemplatePath, new object(),
                    rzv =>
                    {
                        page = rzv; // keep for better errors
                        if (rzv.RazorPage is not IRazor asSxc) return;
                        asSxc.ConnectToRoot(dynCode);
                        // Note: Don't set the purpose here any more, it's a deprecated feature in 12+
                    });
                var writer = new StringWriter();
                await writer.WriteAsync(result);
                return writer;
            }
            catch (Exception maybeIEntityCast)
            {
                throw _errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page);
            }

            // WIP https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs#L397-L404
            // maybe also https://stackoverflow.com/questions/48206993/how-to-load-asp-net-core-razor-view-dynamically-at-runtime
            // later also check loading more DLLs on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime

        }
    }
}