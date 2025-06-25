using ToSic.Sxc.Adam.Sys.Paths;

namespace ToSic.Sxc.Tests.Adam;


public class AdamPathsBaseTests
{
    private static void ThrowIfPathContainsDotDot(string path) => AdamPathsBase.ThrowIfPathContainsDotDot(path);

    [Theory]
    [InlineData("../test")]
    [InlineData("test/../subfolder")]
    public void PathContainsDotDot_ThrowWhenPathIsInValid(string path) =>
        // Act & Assert
        Throws<System.ArgumentException>(() => ThrowIfPathContainsDotDot(path));


    [Theory]
    [InlineData("test/path")]
    [InlineData("test/..path")]
    //[InlineData("test/path..")]
    [InlineData("test/pa..th")]
    [InlineData("test/..path/subfolder")]
    [InlineData("test/path../subfolder")]
    [InlineData("test/pa..th/subfolder")]
    [InlineData("test/path/file.txt")]
    [InlineData("test/path/file..txt")]
    [InlineData("test/path/.gitignore")]
    //[InlineData("test/path/.config.")]
    [InlineData(".file")]
    [InlineData("..other_file")]
    public void PathContainsDotDot_PathIsValid(string path) => ThrowIfPathContainsDotDot(path);
}