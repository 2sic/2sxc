using ToSic.Sxc.Dnn.Razor;

namespace ToSic.Sxc.Dnn.PathCasing;

public class PathCasingValidatorTests
{
    [Theory]
    [InlineData("C:/Projects/2sxc", "should be valid")]
    [InlineData("c:/Projects/2sxc", "should be valid, drive letter ignored")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.json", "should be valid")]
    public void Paths_Valid(string path, string _)
        => True(PathCasingValidator.IsPathCasingExact(path));
    
    [Theory]
    [InlineData("C:/Projects\\2sxc", "incorrect slashes")]
    [InlineData("C:/projects/2sxc", "lower case p")]
    [InlineData("C:/Projects/2sxc/bad.file", "doesn't exist")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.JSON", "UPPER CASE")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.Json", "Upper case Json")]
    public void Paths_NotValid(string path, string _)
        => False(PathCasingValidator.IsPathCasingExact(path));
    
    [Theory]
    [InlineData("C:/Projects/2sxc", "should be valid")]
    [InlineData("c:/Projects/2sxc", "should be valid, drive letter ignored")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.json", "should be valid")]
    public void PathsReversed_Valid(string path, string _)
        => True(PathCasingValidator.IsPathCasingExactReversed(path).IsOk);
    
    [Theory]
    [InlineData("C:/Projects\\2sxc", "incorrect slashes")]
    [InlineData("C:/projects/2sxc", "lower case p")]
    [InlineData("C:/Projects/2sxc/bad.file", "doesn't exist")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.JSON", "UPPER CASE")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.Json", "Upper case Json")]
    public void PathsReversed_NotValid(string path, string _)
        => False(PathCasingValidator.IsPathCasingExactReversed(path).IsOk);
    
    [Theory]
    [InlineData("C:/PROJECTS/2sxc", 0, true, PathCasingValidator.OkMaxSegmentsReached + ": 0", "0 is skip check")]
    [InlineData("C:/PROJECTS/2sxc", 1, true, PathCasingValidator.OkMaxSegmentsReached + ": 1", "should be valid")]
    [InlineData("C:/PROJECTS/2sXc", 1, false, "'2sxc' != '2sXc'", "bad X")]
    [InlineData("C:/PROJECTS/2sxc", 2, false, "'Projects' != 'PROJECTS'", "checks PROJECTS")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.json", 50, true, PathCasingValidator.OkTopReached, "should be valid")]
    [InlineData("C:/Projects/2sxc/2sxc-build.config.JSON", 0, true, PathCasingValidator.OkMaxSegmentsReached + ": 0", "0 is skip check")]
    [InlineData("C:/Projects/2SXC/2sxc-build.config.json", 1, true, PathCasingValidator.OkMaxSegmentsReached + ": 1", "should be valid")]
    [InlineData("C:/Projects/2SXC/2sxc-build.config.json", 2, false, "'2sxc' != '2SXC'", "should be valid")]
    public void PathsReversed_Segments_Valid(string path, int segments, bool expected, string message, string _)
    {
        var test = PathCasingValidator.IsPathCasingExactReversed(path, segments);
        Equal(expected, test.IsOk);
        Equal(message, test.Name);
    }
}
