using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

[Startup(typeof(StartupSxcWithDbPatronPerfectionist))]
public class ImageServiceFormatsPatronPerfectionist(IImageService imgSvc)
    : ImageServiceFormatsBase(imgSvc), IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
{
    protected override int ExpectedPngFormats => 2;
}