namespace ToSic.Sxc.Dnn;

/// <summary>
/// Unit tests for CacheKey value object.
/// Tests validate cache key format, path normalization, hash truncation, and equality behavior.
/// </summary>
public class CacheKeyTests
{
    private const int TestAppId = 42;
    private const string TestEdition = "live";
    private const string TestContentHash = "abcdef1234567890"; // Long hash
    private const string TestAppCodeHash = "xyz789abcdefghij"; // Long hash

    /// <summary>
    /// CacheKey.ToString() format verification
    /// Verifies filename format: {fileName}-{extension}-{contentHash}-{appCodeHash}.dll
    /// </summary>
    [Fact]
    public void CacheKey_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var templatePath = "Views/Default.cshtml";
        var normalizedPath = CacheKeyTestAccessors.NormalizePathTac(templatePath, TestEdition);
        var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, TestContentHash, TestAppCodeHash);

        // Act
        var result = cacheKey.ToStringTac();

        // Assert
        Contains(normalizedPath, result);
        EndsWith(".dll", result);

        // Verify format: {fileName}-{extension}-{hash1}-{hash2}.dll
        var expectedPattern = $"{normalizedPath}";
        StartsWith(expectedPattern, result);
    }

    /// <summary>
    /// Hash truncation verification
    /// Verifies that hashes are truncated to first 6 characters.
    /// </summary>
    [Fact]
    public void CacheKey_ToString_TruncatesHashesToSixCharacters()
    {
        // Arrange
        var templatePath = "Views/Test.cshtml";
        var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, TestContentHash, TestAppCodeHash);

        // Act
        var result = cacheKey.ToStringTac();

        // Assert
        var expectedContentHashShort = TestContentHash.Substring(0, 6); // "abcdef"
        var expectedAppCodeHashShort = TestAppCodeHash.Substring(0, 6); // "xyz789"
        
        Contains(expectedContentHashShort, result);
        Contains(expectedAppCodeHashShort, result);
        
        // Verify full hash is NOT in the filename
        DoesNotContain(TestContentHash, result);
        DoesNotContain(TestAppCodeHash, result);
    }

    /// <summary>
    /// CacheKey.NormalizePath() lowercase conversion
    /// Verifies that paths are converted to lowercase.
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_ConvertsToLowercase()
    {
        // Arrange
        var inputPath = "Views/Default.cshtml";

        // Act
        var result = CacheKeyTestAccessors.NormalizePathTac(inputPath, TestEdition);

        // Assert
        Equal(result, result.ToLowerInvariant());
        DoesNotContain("V", result); // No uppercase V
        DoesNotContain("D", result); // No uppercase D
    }

    /// <summary>
    /// CacheKey.NormalizePath() slash-to-hyphen replacement
    /// Verifies that forward and backslashes are replaced with hyphens.
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_ReplacesSlashesWithHyphens()
    {
        // Arrange
        var inputPath1 = "Views/Default.cshtml";
        var inputPath2 = "Views\\Default.cshtml";

        // Act
        var result1 = CacheKeyTestAccessors.NormalizePathTac(inputPath1, TestEdition);
        var result2 = CacheKeyTestAccessors.NormalizePathTac(inputPath2, TestEdition);

        // Assert
        DoesNotContain("/", result1);
        DoesNotContain("\\", result1);
        DoesNotContain("/", result2);
        DoesNotContain("\\", result2);
        
        Contains("-", result1);
        Contains("-", result2);
        
        // Both should produce same result
        Equal(result1, result2);
    }

    /// <summary>
    /// CacheKey.NormalizePath() .cshtml handling
    /// Verifies that .cshtml extension is converted to -cshtml.
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_HandlesCSHTMLExtension()
    {
        // Arrange
        var inputPath = "Views/Default.cshtml";

        // Act
        var result = CacheKeyTestAccessors.NormalizePathTac(inputPath, TestEdition);

        // Assert
        DoesNotContain(".cshtml", result);
        Contains("-cshtml", result);
    }

    /// <summary>
    /// CacheKey.NormalizePath() complete example
    /// Verifies the documented example: "Views/Default.cshtml" → "views-default-cshtml-6995bfe9"
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_MatchesDocumentedExample()
    {
        // Arrange
        var inputPath = "Views/Default.cshtml";
        var expectedOutput = "views-default-cshtml-6995bfe9";

        // Act
        var result = CacheKeyTestAccessors.NormalizePathTac(inputPath, TestEdition);

        // Assert
        Equal(expectedOutput, result);
    }

    /// <summary>
    /// CacheKey.NormalizePath() removes app root prefix such as "Portals/.../2sxc/app".
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_TrimsAppRootPrefix()
    {
        // Arrange
        var inputPath = "Portals/0/2sxc/MyApp/live/views/Home.cshtml";
        var expectedOutput = "views-home-cshtml-e5954f02";

        // Act
        var result = CacheKeyTestAccessors.NormalizePathTac(inputPath, TestEdition);

        // Assert
        Equal(expectedOutput, result);
        DoesNotContain("live-", result);
    }

    /// <summary>
    /// CacheKey.NormalizePath() removes edition segment when it appears at the beginning of the app-scoped path.
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_TrimsEditionPrefix()
    {
        // Arrange
        const string inputPath = "bs5/textimage/text-and-image.cshtml";
        const string edition = "bs5";
        const string expectedOutput = "textimage-text-and-image-cshtml-8c705c9f";

        // Act
        var result = CacheKeyTestAccessors.NormalizePathTac(inputPath, edition);

        // Assert
        Equal(expectedOutput, result);
        DoesNotContain($"{edition}-", result);
    }

    /// <summary>
    /// CacheKey.NormalizePath() uses provided app path instead of relying on hard-coded path fragments.
    /// Ensures app root trimming works even when the app folder name matches the default marker.
    /// </summary>
    [Fact]
    public void CacheKey_NormalizePath_TrimsUsingProvidedAppPath()
    {
        // Arrange
        const string inputPath = "2sxc/2sxc/views/Home.cshtml";
        const string appPath = "2sxc/2sxc";

        // Act
        var result = CacheKeyTestAccessors.NormalizePathTac(inputPath, edition: "root", appPath: appPath);

        // Assert
        StartsWith("views-home-cshtml-", result);
    }

    /// <summary>
    /// Verifies that CacheKey equality works correctly for identical values.
    /// </summary>
    [Fact]
    public void CacheKey_Equals_ReturnsTrueForIdenticalKeys()
    {
        // Arrange
        var templatePath = "Views/Test.cshtml";
        var key1 = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, TestContentHash, TestAppCodeHash);
        var key2 = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, TestContentHash, TestAppCodeHash);

        // Act & Assert
        Equal(key1, key2);
        True(key1.EqualsTac(key2));
        Equal(key1.GetHashCodeTac(), key2.GetHashCodeTac());
    }

    /// <summary>
    /// Verifies that CacheKey equality returns false for different values.
    /// </summary>
    [Fact]
    public void CacheKey_Equals_ReturnsFalseForDifferentKeys()
    {
        // Arrange
        var templatePath = "Views/Test.cshtml";
        var key1 = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, TestContentHash, TestAppCodeHash);
        var key2 = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, "different", TestAppCodeHash);

        // Act & Assert
        NotEqual(key1, key2);
        False(key1.EqualsTac(key2));
    }

    /// <summary>
    /// Verifies that GetFilePath combines cache directory with filename correctly.
    /// </summary>
    [Fact]
    public void CacheKey_GetFilePath_CombinesDirectoryAndFilename()
    {
        // Arrange
        var cacheDir = @"C:\temp\cache";
        var templatePath = "Views/Test.cshtml";
        var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, templatePath, TestContentHash, TestAppCodeHash);

        // Act
        var result = cacheKey.GetFilePathTac(cacheDir);

        // Assert
        StartsWith(cacheDir, result);
        EndsWith(".dll", result);
        Contains(Path.DirectorySeparatorChar.ToString(), result);
    }

    /// <summary>
    /// Verifies that constructor validates parameters.
    /// </summary>
    [Theory]
    [InlineData(0, "live", "path", "hash1", "hash2")] // Invalid appId
    [InlineData(-1, "live", "path", "hash1", "hash2")] // Negative appId
    public void CacheKey_Constructor_ThrowsOnInvalidAppId(int appId, string edition, string path, string hash1, string hash2)
    {
        // Act & Assert
        Throws<ArgumentException>(() => CacheKeyTestAccessors.NewCacheKeyTac(appId, edition, path, hash1, hash2));
    }

    /// <summary>
    /// Verifies that constructor defaults null/empty edition to "root".
    /// Spec: Edition parameter is nullable and defaults to "root" if not provided.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CacheKey_Constructor_DefaultsEditionToRoot(string? edition)
    {
        // Arrange
        var templatePath = "Views/Test.cshtml";

        // Act
        var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, edition, templatePath, TestContentHash, TestAppCodeHash);

        // Assert
        Equal("root", cacheKey.Edition);
    }

    /// <summary>
    /// Verifies that constructor uses provided edition when non-empty.
    /// </summary>
    [Theory]
    [InlineData("live")]
    [InlineData("staging")]
    [InlineData("draft")]
    public void CacheKey_Constructor_UsesProvidedEdition(string edition)
    {
        // Arrange
        var templatePath = "Views/Test.cshtml";

        // Act
        var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, edition, templatePath, TestContentHash, TestAppCodeHash);

        // Assert
        Equal(edition, cacheKey.Edition);
    }
}
