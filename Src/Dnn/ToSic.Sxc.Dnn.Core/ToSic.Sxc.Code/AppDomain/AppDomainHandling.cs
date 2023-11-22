using System;
using System.IO;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    internal abstract class AppDomainHandling
    {
        protected AppDomain CustomAppDomain { get; set; }

        protected AppDomain CreateNewAppDomain(string appDomain)
        {
            var domainSetup = new AppDomainSetup
            {
                ApplicationName = appDomain,
                ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin") // current bin directory
            };
            return AppDomain.CreateDomain(appDomain, null, domainSetup);
        }

        protected void Unload()
        {
            if (CustomAppDomain == null) return;
            AppDomain.Unload(CustomAppDomain);
            CustomAppDomain = null;
        }
    }
}
