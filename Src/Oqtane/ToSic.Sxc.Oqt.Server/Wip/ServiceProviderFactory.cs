//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting.Internal;

//namespace ToSic.Sxc.Oqt.Server.Wip
//{
//    public static class ServiceProviderFactory
//    {
//        public static IServiceProvider ServiceProvider { get; }

//        static ServiceProviderFactory()
//        {
//            IWebHostEnvironment env = new HostEnvironment();
//            env.ContentRootPath = Directory.GetCurrentDirectory();
//            env.EnvironmentName = "Development";

//            Startup startup = new Startup();
//            startup.HostEnvironment = env;
//            ServiceCollection sc = new ServiceCollection();
//            startup.ConfigureServices(sc);
//            ServiceProvider = sc.BuildServiceProvider();
//        }
//    }
//}
