using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class ImageServiceFormatsPatronPerfectionist : ImageServiceFormatsBase
{
    // Start the test with a platform-info that has a patron
    protected override void SetupServices(IServiceCollection services)
    {
        base.SetupServices(services);
        services.AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();
    }


    protected override int ExpectedPngFormats => 2;
}