using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
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
            var contentRootPath = Path.GetFullPath(Path.Combine(webHost.ContentRootPath, OqtConstants.AppRoot));

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
            //services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            //    {
            //        options.FileProviders.Add(new PhysicalFileProvider(contentRootPath));
            //        foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll"))
            //            options.AdditionalReferencePaths.Add(dllFile);
            //    });
            //services.AddRazorPages();


            //var sd1 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IConfigureOptions<MvcRazorRuntimeCompilationOptions>));
            //if (sd1 != null) 
            //    services.Remove(sd1);

            //var sd2 = services.FirstOrDefault(f =>
            //    f.ServiceType == typeof(IViewCompilerProvider) &&
            //    f.ImplementationType?.Assembly == typeof(IViewCompilerProvider).Assembly &&
            //    f.ImplementationType.FullName == "Microsoft.AspNetCore.Mvc.Razor.Compilation.DefaultViewCompilerProvider");
            //if (sd2 != null)
            //    services.Remove(sd2);

            //var sd3 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IConfigureOptions<IViewCompilerProvider>));
            //if (sd3 != null)
            //    services.Remove(sd3);

            //var sd4 = services.FirstOrDefault(f =>
            //    f.ServiceType == typeof(IActionDescriptorProvider) &&
            //    f.ImplementationType == typeof(CompiledPageActionDescriptorProvider));
            //if (sd4 != null)
            //    services.Remove(sd4);

            //var sd5 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IActionDescriptorProvider));
            //if (sd5 != null)
            //    services.Remove(sd5);

            //var sd6 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(MatcherPolicy));
            //if (sd6 != null)
            //    services.Remove(sd6);

            //var sd7 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(RazorProjectFileSystem));
            //if (sd7 != null)
            //    services.Remove(sd7);

            //var sd8 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(RazorProjectEngine));
            //if (sd8 != null)
            //    services.Remove(sd8);

            //var sd9 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IPageRouteModelProvider));
            //if (sd9 != null) 
            //    services.Remove(sd9);

            //var sd10 = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IActionDescriptorChangeProvider));
            //if (sd10 != null) 
            //    services.Remove(sd10);




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
