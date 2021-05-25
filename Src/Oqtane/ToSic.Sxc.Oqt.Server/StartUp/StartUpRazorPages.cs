using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    /// <summary>
    /// Still experimental. Getting Razor Pages to compile seems very, very difficult
    /// Because we probably have quite a bit of code, we'll try to put it here
    /// </summary>
    internal class StartUpRazorPages
    {

        public void ConfigureServices(IServiceCollection services)
        {
            // get path for content
            var tempDi = services.BuildServiceProvider();
            var webHost = tempDi.Build<IWebHostEnvironment>();
            var contentRootPath = Path.GetFullPath(Path.Combine(webHost.ContentRootPath, OqtConstants.ContentSubfolder));

            //// Add razor pages dynamic compilation WIP
            var dllLocation = typeof(Oqtane.Server.Program).Assembly.Location;
            var dllPath = Path.GetDirectoryName(dllLocation);
            var mvcBuilder = services.AddRazorPages()
                // experiment
                // https://github.com/aspnet/samples/blob/master/samples/aspnetcore/mvc/runtimecompilation/MyApp/Startup.cs#L26
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider(contentRootPath));
                    foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll"))
                        options.AdditionalReferencePaths.Add(dllFile);

                });

            // exp: access the anti forgery so it's loaded
            //var temp = new Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions();
            //mvcBuilder.AddApplicationPart(typeof(Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions).Assembly);
            //LoadAssembliesForRazor(mvcBuilder, dllPath);
            //var builderDebug = mvcBuilder.PartManager;
        }



        //// Experimental - taken from an old comming Nov 6 2019 from https://github.com/RickStrahl/Westwind.AspnetCore.LiveReload/
        //// newer from https://github.com/RickStrahl/LiveReloadServer/blob/master/LiveReloadServer/Startup.cs
        //private string WebRoot;
        //private List<string> LoadedPrivateAssemblies = new List<string>();
        //private List<string> FailedPrivateAssemblies = new List<string>();

        //private void LoadAssembliesForRazor(IMvcBuilder mvcBuilder, string binPath)
        //{
        //    //var binPath = Path.Combine(WebRoot, "privatebin");
        //    if (Directory.Exists(binPath))
        //    {
        //        var files = Directory.GetFiles(binPath);
        //        foreach (var file in files.Where(f => f.Contains("antiforgery", StringComparison.InvariantCultureIgnoreCase)))
        //        {
        //            if (!file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)
        //                //&& !file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)
        //            ) continue;

        //            try
        //            {
        //                var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
        //                mvcBuilder.AddApplicationPart(asm);

        //                LoadedPrivateAssemblies.Add(file);
        //            }
        //            catch (Exception ex)
        //            {
        //                FailedPrivateAssemblies.Add(file + "\n    - " + ex.Message);
        //            }

        //        }
        //    }

        //}

    }
}
