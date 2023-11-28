using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Tests.Adam.Imageflow
{
    [TestClass()]
    public class OqtaneBlobServiceTests
    {
        private static readonly OqtaneBlobService OqtaneBlobService = new(null);
        private static bool SupportsPath(string virtualPath) => OqtaneBlobService.SupportsPath(virtualPath);  
        private static bool GetAppNameAndFilePath(string virtualPath, out string appName, out string filePath) => OqtaneBlobService.GetAppNameAndFilePath(virtualPath, out appName, out filePath);
        private static bool ContainsSxcPath(string virtualPath) => OqtaneBlobService.ContainsSxcPath(virtualPath);

        [TestMethod()]
        [DataRow("app/Content/adam/2aUKYsXls0KCeVH-XnHmRQ/Image/Demo-1.jpg?w=600&h=370&quality=75&mode=crop&scale=both")]
        [DataRow("app/Gallery7/adam/0jPONC8CT0i06sp3dybtCw/Images/boris-baldinger-eUFfY6cwjSU-unsplash.jpg")]
        [DataRow("app/Content/adam/2aUKYsXls0KCeVH-XnHmRQ/Image/Demo-1.jpg?w=2000&amp;h=1500&amp;quality=75&amp;mode=max&amp;scale=downscaleonly")]
        [DataRow("app/Gallery7/adam/0jPONC8CT0i06sp3dybtCw/Images/boris-baldinger-eUFfY6cwjSU-unsplash.jpg")]
        public void SupportsPathTest(string virtualPath) => Assert.IsTrue(SupportsPath(virtualPath));

        [TestMethod()]
        [DataRow($"Modules/{OqtConstants.PackageName}/assets/app-primary.png")]
        public void NotSupportsPathTest(string virtualPath) => Assert.IsFalse(SupportsPath(virtualPath));

        [TestMethod()]
        [DataRow("app/Tutorial-Razor/assets/app-icon.png", "Tutorial-Razor", "app-icon.png")]
        [DataRow("child-site/en-uk/app/Tutorial-Razor/assets/folder/app/subfolder/assets/app-icon.png", "Tutorial-Razor", "folder/app/subfolder/assets/app-icon.png")]
        public void GetAppNameAndFilePathTest(string virtualPath, string expectedAppName, string expectedFilePath)
        {
            Assert.IsTrue(GetAppNameAndFilePath(virtualPath, out var actualAppName, out var actualFilePath));
            Assert.AreEqual(expectedAppName, actualAppName);
            Assert.AreEqual(expectedFilePath, actualFilePath);
        }

        [TestMethod()]
        [DataRow($"Modules/{OqtConstants.PackageName}/assets/app-primary.png")]
        public void NotSupportedGetAppNameAndFilePathTest(string virtualPath) => Assert.IsFalse(GetAppNameAndFilePath(virtualPath, out var actualAppName, out var actualFilePath));

        [TestMethod()]
        [DataRow("app/Tutorial-Razor/assets/app-icon.png")]
        [DataRow("1/app/Tutorial_-1230Razor/assets/1/app-icon.png")]
        [DataRow("1/2/app/Tutorial-Razor/assets/1/2/asset/app/app-icon.png")]
        public void ContainsSxcPathPathTest(string virtualPath) => Assert.IsTrue(ContainsSxcPath(virtualPath));

        [TestMethod()]
        [DataRow($"Modules/{OqtConstants.PackageName}/assets/app-primary.png")]
        [DataRow("app/Tutori/al/Ra/zor/assets/app-icon.png")]
        [DataRow("app/Content/adam/2aUKYsXls0KCeVH-XnHmRQ/Image/Demo-1.jpg?w=2000&h=1500&quality=75&mode=max&scale=downscaleonly")]
        public void NotContainsSxcPathTest(string virtualPath) => Assert.IsFalse(ContainsSxcPath(virtualPath));
    }
}