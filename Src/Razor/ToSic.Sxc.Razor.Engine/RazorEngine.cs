using System;
using System.IO;
using System.Threading.Tasks;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Hybrid.Razor;

namespace ToSic.Sxc.Razor.Engine
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]

    public partial class RazorEngine : EngineBase
    {
        #region Constructor / DI

        public RazorEngine(EngineBaseDependencies helpers) : base(helpers) { }
        
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
                var dynCode = new Code.DynamicCodeRoot().Init(Block, Log);

                var compiler = Eav.Factory.Resolve<IRazorRenderer>();
                var result = await compiler.RenderToStringAsync(TemplatePath, new object(),
                    rzv =>
                    {
                        if (rzv.RazorPage is ISxcRazorComponent asSxc)
                        {
                            asSxc.DynCode = dynCode;
                            asSxc.Purpose = Purpose;

                        }
                    });
                var writer = new StringWriter();
                await writer.WriteAsync(result);
                return writer;
            }
            catch (Exception maybeIEntityCast)
            {
                Code.ErrorHelp.AddHelpIfKnownError(maybeIEntityCast);
                throw;
            }

            // WIP https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs#L397-L404
            // maybe also https://stackoverflow.com/questions/48206993/how-to-load-asp-net-core-razor-view-dynamically-at-runtime
            // later also check loading more DLLs on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime

        }
    }
}