using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class ImageServiceFormatsPatronPerfectionist : ImageServiceFormatsBase
{
    // Start the test with a platform-info that has a patron
    protected override IServiceCollection SetupServices(IServiceCollection services) =>
        base.SetupServices(services)
            .AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();


    protected override int ExpectedPngFormats => 2;
}