using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

[Startup(typeof(StartupSxcWithDbBasic))]
public class ImageServiceFormatsNoPatron(IImageService imgSvc)
    : ImageServiceFormatsBase(imgSvc), IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    protected override int ExpectedPngFormats => 0;
}