using System;
using System.IO;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.Compile.AppDomain;

[PrivateApi]
internal abstract class AppDomainHandling
{
    protected System.AppDomain CustomAppDomain { get; set; }

    protected System.AppDomain CreateNewAppDomain(string appDomain)
    {
        var domainSetup = new AppDomainSetup
        {
            ApplicationName = appDomain,
            ApplicationBase = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin") // current bin directory
        };
        return System.AppDomain.CreateDomain(appDomain, null, domainSetup);
    }

    protected void Unload()
    {
        if (CustomAppDomain == null) return;
        System.AppDomain.Unload(CustomAppDomain);
        CustomAppDomain = null;
    }
}