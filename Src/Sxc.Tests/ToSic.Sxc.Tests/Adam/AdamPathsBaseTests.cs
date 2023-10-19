using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Adam;
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
        public void ThrowIfPathContainsDotDot_WhenPathIsInValid(string path) =>
            // Act & Assert
            ThrowsException<System.ArgumentException>(() => ThrowIfPathContainsDotDot(path));


        [DataTestMethod]
        [DataRow("test/path")]
        [DataRow("test/path/file.txt")]
        [DataRow("test/path/file..txt")]
        [DataRow("test/path/.gitignore")]
        [DataRow("test/path/.config.")]
        [DataRow(".file")]
        [DataRow("..other_file")]
        public void ThrowIfPathContainsDotDot_WhenPathIsValid(string path) => ThrowIfPathContainsDotDot(path);
    }
}