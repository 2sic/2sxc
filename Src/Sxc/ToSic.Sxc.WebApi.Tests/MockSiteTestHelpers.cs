using ToSic.Eav.Apps.Mocks;

namespace ToSic.Sxc.WebApi.Tests;

internal static class MockSiteTestHelpers
{
    /// <summary>
    /// Create a minimal reusable site model for WebApi tests.
    /// </summary>
    public static MockSite CreateSite(string appRoot)
        => new()
        {
            Id = 1,
            Name = "Test",
            AppsRootPhysical = appRoot,
            AppsRootPhysicalFull = appRoot,
            AppAssetsLinkTemplate = "/app/{appFolder}",
            ContentPath = "/",
            Url = "/",
            UrlRoot = "/",
            CurrentCultureCode = "en-us",
            DefaultCultureCode = "en-us",
            ZoneId = 1,
        };
}
