using System;
using System.IO;
using System.Threading.Tasks;
using Custom.Hybrid;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]

    public partial class RazorEngine : EngineBase, IRazorEngine
    {
        private readonly Lazy<DynamicCodeRoot> _dynCodeRootLazy;
        public IRazorRenderer RazorRenderer { get; }

        #region Constructor / DI

        public RazorEngine(EngineBaseDependencies helpers, IRazorRenderer razorRenderer, Lazy<DynamicCodeRoot> dynCodeRootLazy) : base(helpers)
        {
            _dynCodeRootLazy = dynCodeRootLazy;
            RazorRenderer = razorRenderer;
        }
        
        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            // in MVC & Oqtane we're always using V10 compatibility
            CompatibilityAutoLoadJQueryAndRVT = false;
        }

        #endregion
        
        /// <inheritdoc/>
        protected override string RenderTemplate()
        {
            Log.Call();
            var task = RenderTask();
            task.Wait();
            return task.Result.ToString();
        }

        [PrivateApi]
        public async Task<TextWriter> RenderTask()
        {
            Log.Add("will render into TextWriter");
            try
            {
                if (string.IsNullOrEmpty(TemplatePath)) return null;
                var dynCode = _dynCodeRootLazy.Value.Init(Block, Log, Constants.CompatibilityLevel12);

                var result = await RazorRenderer.RenderToStringAsync(TemplatePath, new object(),
                    rzv =>
                    {
                        if (rzv.RazorPage is not IRazor asSxc) return;
                        asSxc.DynamicCodeCoupling(dynCode);
                        // TODO: purpose may be missing?
                        //asSxc.Purpose = Purpose;
                    });
                var writer = new StringWriter();
                await writer.WriteAsync(result);
                return writer;
            }
            catch (Exception maybeIEntityCast)
            {
                ErrorHelp.AddHelpIfKnownError(maybeIEntityCast);
                throw;
            }

            // WIP https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs#L397-L404
            // maybe also https://stackoverflow.com/questions/48206993/how-to-load-asp-net-core-razor-view-dynamically-at-runtime
            // later also check loading more DLLs on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime

        }
    }
}