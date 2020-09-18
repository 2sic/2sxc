using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Mvc.Code;
using ToSic.Sxc.Mvc.RazorPages;

namespace ToSic.Sxc.Mvc.Engines
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]

    public partial class MvcRazorEngine : EngineBase
    {
        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            // in MVC we're always using V10 compatibility
            CompatibilityAutoLoadJQueryAndRVT = false;

            try
            {
                InitWebpage();
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (ConfigurationErrorsException exc)
            {
                var e = new Exception("Configuration Error: Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
                throw e;
            }
        }

        [PrivateApi]
        public async Task<TextWriter> RenderTask()
        {
            Log.Add("will render into TextWriter");
            try
            {
                if (string.IsNullOrEmpty(TemplatePath)) return null;
                var dynCode = new MvcDynamicCode().Init(Block, 10, Log);

                var compiler = Eav.Factory.Resolve<IRenderRazor>();
                var result = await compiler.RenderToStringAsync(TemplatePath, new Object(),
                    rzv =>
                    {
                        if (rzv.RazorPage is IIsSxcRazorPage asSxc)
                        {
                            asSxc.DynCode = dynCode;
                            asSxc.VirtualPath = TemplatePath;
                            asSxc.Purpose = Purpose;

                        }
                    });
                var writer = new StringWriter();
                writer.Write(result);
                // todo: continue here 2020-08-19
                return writer;
            }
            catch (Exception maybeIEntityCast)
            {
                Sxc.Code.ErrorHelp.AddHelpIfKnownError(maybeIEntityCast);
                throw;
            }
        }



        /// <inheritdoc/>
        protected override string RenderTemplate()
        {
            Log.Call();
            var task = RenderTask();
            task.Wait();
            return task.Result.ToString();
        }

        private string InitWebpage()
        {
            if (string.IsNullOrEmpty(TemplatePath)) return null;
            var dynCode = new MvcDynamicCode().Init(Block, 10, Log);

            var compiler = Eav.Factory.Resolve<IRenderRazor>();
            var result = compiler.RenderToStringAsync(TemplatePath, new Object(), 
                rzv =>
                {
                    if (rzv.RazorPage is IIsSxcRazorPage asSxc)
                    {
                        asSxc.DynCode = dynCode;
                        asSxc.VirtualPath = TemplatePath;
                        asSxc.Purpose = Purpose;

                    }

                });
            // todo: de-async!
            return result.Result;

            // WIP https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs#L397-L404
            // maybe also https://stackoverflow.com/questions/48206993/how-to-load-asp-net-core-razor-view-dynamically-at-runtime
            // later also check loading more DLLs on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime

        }
    }
}