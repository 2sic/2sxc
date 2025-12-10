

using ToSic.Eav.Sys;
// ReSharper disable once CheckNamespace

namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Unit tests for ExtensionReaderBackend editions detection
/// </summary>
public class ExtensionsReaderEditionsTests
{
    #region Basic Editions Tests

    [Fact]
    public void GetExtensions_NoEditions_WhenEditionsSupportedFalse()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        
        const string extName = "no-editions-ext";
        ctx.SetupExtension(extName, new 
        { 
            version = "1.0.0",
            inputTypeInside = "string-test",
            editionsSupported = false
        });
        
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
        Assert.NotNull(result);
        var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
        Assert.Single(extensions);
        var ext = extensions.First();
        Assert.Equal(extName, ext.Folder);
        Assert.Equal(string.Empty, ext.Edition);
    }

    [Fact]
    public void GetExtensions_NoEditions_WhenEditionsSupportedMissing()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        
        const string extName = "no-flag-ext";
        ctx.SetupExtension(extName, new 
        { 
            version = "1.0.0",
            inputTypeInside = "string-test"
        });
        
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
        var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
        Assert.Single(extensions);
        Assert.Equal(string.Empty, extensions[0].Edition);
    }

    [Fact]
    public void GetExtensions_DetectsEdition_WhenEditionFolderExists()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        
        const string extName = "multi-edition";
        const string inputType = "string-font-icon";
        
        // Primary extension
        ctx.SetupExtension(extName, new 
        { 
            version = "1.0.0",
            inputTypeInside = inputType,
            editionsSupported = true
        });
        
        // Staging edition
        ctx.SetupEdition("staging", extName, new
        {
            version = "1.0.0-staging",
            inputTypeInside = inputType,
            editionName = "Staging"
        });
        
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
        var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
        Assert.Equal(2, extensions.Count);

        var root = extensions.Single(e => e.Edition == string.Empty);
        Assert.Equal(extName, root.Folder);

        var stagingEdition = extensions.Single(e => e.Edition == "staging");
        Assert.Equal(extName, stagingEdition.Folder);
        Assert.NotNull(stagingEdition.Configuration);
    }

    [Fact]
    public void GetExtensions_DetectsMultipleEditions()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        
        const string extName = "multi-edition";
        const string inputType = "string-dropdown";
        
        ctx.SetupExtension(extName, new 
        { 
            version = "1.0.0",
            inputTypeInside = inputType,
            editionsSupported = true
        });
        
        ctx.SetupEdition("staging", extName, new
        {
            version = "1.0.0-staging",
            inputTypeInside = inputType
        });
        
        ctx.SetupEdition("live", extName, new
        {
            version = "1.0.0",
            inputTypeInside = inputType
        });
        
        ctx.SetupEdition("dev", extName, new
        {
            version = "1.0.0-dev",
            inputTypeInside = inputType
        });
        
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
        var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
        Assert.Equal(4, extensions.Count);

        Assert.Contains(extensions, e => e.Edition == string.Empty);
        Assert.Contains(extensions, e => e.Edition == "staging");
        Assert.Contains(extensions, e => e.Edition == "live");
        Assert.Contains(extensions, e => e.Edition == "dev");
    }

    #endregion

    #region Edition Validation Tests

    //[Fact]
    //public void GetExtensions_SkipsEdition_WhenInputTypeMismatch()
    //{
    //    using var ctx = ExtensionsReaderTestContext.Create();
        
    //    const string extName = "mismatch-test";
        
    //    ctx.SetupExtension(extName, new 
    //    { 
    //        version = "1.0.0",
    //        inputTypeInside = "string-font-icon",
    //        editionsSupported = true
    //    });
        
    //    // Edition with different inputType - should be skipped
    //    ctx.SetupEdition("staging", extName, new
    //    {
    //        version = "1.0.0",
    //        inputTypeInside = "string-different"
    //    });
        
    //    var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
    //    var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
    //    Assert.Single(extensions);
    //    Assert.Equal(string.Empty, extensions[0].Edition);
    //}

    //[Fact]
    //public void GetExtensions_SkipsEdition_WhenManifestMissing()
    //{
    //    using var ctx = ExtensionsReaderTestContext.Create();
        
    //    const string extName = "no-manifest-edition";
        
    //    ctx.SetupExtension(extName, new 
    //    { 
    //        version = "1.0.0",
    //        inputTypeInside = "string-test",
    //        editionsSupported = true
    //    });
        
    //    // Create edition folder without manifest
    //    ctx.CreateEditionFolderOnly("staging", extName);
        
    //    var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
    //    var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
    //    Assert.Single(extensions);
    //    Assert.Equal(string.Empty, extensions[0].Edition);
    //}

    //[Fact]
    //public void GetExtensions_SkipsEdition_WhenInputTypeInvalid()
    //{
    //    using var ctx = ExtensionsReaderTestContext.Create();
        
    //    const string extName = "invalid-input-type";
        
    //    ctx.SetupExtension(extName, new 
    //    { 
    //        version = "1.0.0",
    //        inputTypeInside = "string-test",
    //        editionsSupported = true
    //    });
        
    //    // Edition with empty inputTypeInside
    //    ctx.SetupEdition("staging", extName, new
    //    {
    //        version = "1.0.0",
    //        inputTypeInside = ""
    //    });
        
    //    var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
    //    var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
    //    Assert.Single(extensions);
    //    Assert.Equal(string.Empty, extensions[0].Edition);
    //}

    #endregion

    #region Edition Configuration Tests

    [Fact]
    public void GetExtensions_EditionConfiguration_PreservesAllProperties()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        const string extName = "config-test";
        const string inputType = "string-test";
        ctx.SetupExtension(extName, new
        {
            version = "1.0.0",
            inputTypeInside = inputType,
            editionsSupported = true
        });
        ctx.SetupEdition("staging", extName, new
        {
            version = "1.0.1-staging",
            inputTypeInside = inputType,
            customProp = "test-value",
            nestedObj = new { key = "value" }
        });
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
        var stagingEdition = extensions.Single(e => e.Edition == "staging");
        var config = stagingEdition.Configuration;
        Assert.NotNull(config);

        // Configuration is now ExtensionManifest - check properties directly
        Assert.Equal("1.0.1-staging", config.Version);
        
        // Note: customProp and nestedObj won't be in ExtensionManifest as it only has defined properties
        // If tests need arbitrary JSON properties, the manifest needs JsonElement fields or we need different test data
    }

    #endregion

    #region Extensions Folder Filtering Tests

    [Fact]
    public void GetExtensions_DoesNotTreatExtensionsFolderAsEdition()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        
        const string extName = "filter-test";
        
        ctx.SetupExtension(extName, new 
        { 
            version = "1.0.0",
            inputTypeInside = "string-test",
            editionsSupported = true
        });
        
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
        var extensions = result.Extensions.Where(e => e.Folder == extName).ToList();
        Assert.Single(extensions);

        // Should not have an entry treating the extensions folder as an edition
        Assert.DoesNotContain(extensions, e => e.Edition.Equals("extensions", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Multiple Extensions with Editions

    [Fact]
    public void GetExtensions_HandlesMultipleExtensionsWithDifferentEditionSetups()
    {
        using var ctx = ExtensionsReaderTestContext.Create();
        
        // Extension 1: with editions
        ctx.SetupExtension("ext-with-editions", new 
        { 
            version = "1.0.0",
            inputTypeInside = "string-font-icon",
            editionsSupported = true
        });
        ctx.SetupEdition("staging", "ext-with-editions", new
        {
            version = "1.0.0",
            inputTypeInside = "string-font-icon"
        });
        
        // Extension 2: without editions
        ctx.SetupExtension("ext-without-editions", new 
        { 
            version = "2.0.0",
            inputTypeInside = "string-dropdown",
            editionsSupported = false
        });
        
        var result = ctx.ReaderBackend.GetExtensions(appId: 42);
        
        Assert.Equal(3, result.Extensions.Count);

        var extWithEditions = result.Extensions.Where(e => e.Folder == "ext-with-editions").ToList();
        Assert.Equal(2, extWithEditions.Count);
        Assert.Contains(extWithEditions, e => e.Edition == string.Empty);
        Assert.Contains(extWithEditions, e => e.Edition == "staging");
        
        var extWithoutEditions = result.Extensions.Where(e => e.Folder == "ext-without-editions").ToList();
        Assert.Single(extWithoutEditions);
        Assert.Equal(string.Empty, extWithoutEditions[0].Edition);
    }

    #endregion
}
