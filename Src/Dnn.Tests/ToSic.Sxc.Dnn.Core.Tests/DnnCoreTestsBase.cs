using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Configuration;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Core.Tests.Web;
using ToSic.Sxc.Startup;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Dnn.Core.Tests
{
    public class DnnCoreTestsBase: TestBaseEav
    {
        public DnnCoreTestsBase()
        {
            // Make sure that features are ready to use
            var sysLoader = GetService<EavSystemLoader>();
            sysLoader.ReloadFeatures();
        }

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            //services.AddSxcCore();
            services.TryAddTransient<CodeCompiler, CodeCompilerNetFull>();
            services.TryAddSingleton<IHostingEnvironmentWrapper, HostingEnvironmentMock> ();
            services.TryAddSingleton<IReferencedAssembliesProvider, ReferencedAssembliesProviderMock > ();
        }

        protected dynamic CreateInstance(Assembly assembly, string name)
        {
            var compiledType = assembly.GetType(name);
            dynamic instance = Build<object>(compiledType, Log);
            return instance;
        }
    }
}
