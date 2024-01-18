using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.Adam
{
    [TestClass]
    public class AdamPathsBaseTests
    {
        private static void ThrowIfPathContainsDotDot(string path) => AdamPathsBase.ThrowIfPathContainsDotDot(path);

        [DataTestMethod]
        [DataRow("../test")]
        [DataRow("test/../subfolder")]
        public void PathContainsDotDot_ThrowWhenPathIsInValid(string path) =>
            // Act & Assert
            ThrowsException<System.ArgumentException>(() => ThrowIfPathContainsDotDot(path));


        [DataTestMethod]
        [DataRow("test/path")]
        [DataRow("test/..path")]
        //[DataRow("test/path..")]
        [DataRow("test/pa..th")]
        [DataRow("test/..path/subfolder")]
        [DataRow("test/path../subfolder")]
        [DataRow("test/pa..th/subfolder")]
        [DataRow("test/path/file.txt")]
        [DataRow("test/path/file..txt")]
        [DataRow("test/path/.gitignore")]
        //[DataRow("test/path/.config.")]
        [DataRow(".file")]
        [DataRow("..other_file")]
        public void PathContainsDotDot_PathIsValid(string path) => ThrowIfPathContainsDotDot(path);
    }
}