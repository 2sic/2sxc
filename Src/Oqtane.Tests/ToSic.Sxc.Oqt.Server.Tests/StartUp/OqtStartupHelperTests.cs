using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.StartUp.Tests
{
    [TestClass]
    public class OqtStartupHelperTests
    {
        private static bool IsSxcEndpoint(string path) => OqtStartupHelper.IsSxcEndpoint(path);
        private static bool IsSxcDialog(string path) => OqtStartupHelper.IsSxcDialog(path);

        [DataTestMethod]
        [DataRow("app/testApp/api/testController/testAction")]
        [DataRow("app/testApp/edition/api/testController/testAction")]
        [DataRow("path/app/testApp/api/testController/testAction")]
        [DataRow("path/app/testApp/edition/api/testController/testAction")]
        [DataRow("path/subpath/app/testApp/api/testController/testAction")]
        [DataRow("path/subpath/app/testApp/edition/api/testController/testAction")]
        [DataRow("subsite/path/subpath/app/testApp/api/testController/testAction")]
        [DataRow("subsite/path/subpath/app/testApp/edition/api/testController/testAction")]
        public void IsSxcEndpoint_ShouldReturnTrue_ForValidPath(string validPath)
            => Assert.IsTrue(IsSxcEndpoint(validPath));

        [DataTestMethod]
        [DataRow("invalid/path/to/api")]
        [DataRow("app/testApp/api")]
        [DataRow("path/app/testApp/api/")]
        [DataRow("path/app/api/testController/testAction")]
        [DataRow("path/app//api/testController/testAction")]
        [DataRow("testApp/api/testController/testAction")]
        [DataRow("app/testApp/not-api/testController/testAction")]
        [DataRow("app/testApp/edition/not-api/testController/testAction")]
        [DataRow("path/app/testApp//api/testController/testAction")]
        [DataRow("path/app/testApp/not-api/testController/testAction")]
        [DataRow("path/app/testApp/edition/not-api/testController/testAction")]
        [DataRow("path/subpath/app/testApp/not-api/testController/testAction")]
        [DataRow("path/subpath/app/testApp/edition/not-api/testController/testAction")]
        [DataRow("subsite/path/subpath/app/testApp/not-api/testController/testAction")]
        [DataRow("subsite/path/subpath/app/testApp/edition/not-api/testController/testAction")]
        [DataRow("not-only-app/testApp/api")]
        public void IsSxcEndpoint_ShouldReturnFalse_ForInvalidPath(string invalidPath)
            => Assert.IsFalse(IsSxcEndpoint(invalidPath));

        [DataTestMethod]
        [DataRow("/Modules/" + OqtConstants.PackageName + "/dist/quick-dialog/")]
        [DataRow("/Modules/" + OqtConstants.PackageName + "/dist/ng-edit/")]
        public void IsSxcDialog_ShouldReturnTrue_ForValidPath(string validPath)
            => Assert.IsTrue(IsSxcDialog(validPath));

        [DataTestMethod]
        [DataRow("/invalid/path/to/dialog")]
        [DataRow("/Modules/InvalidPackageName/dist/quick-dialog/")]
        [DataRow("/Modules/InvalidPackageName/dist/ng-edit/")]
        public void IsSxcDialog_ShouldReturnFalse_ForInvalidPath(string invalidPath)
            => Assert.IsFalse(IsSxcDialog(invalidPath));
    }
}