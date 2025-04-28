using ToSic.Sxc.Oqt.Server.StartUp;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.StartUp;

public class OqtStartupHelperTests
{
    private static bool IsSxcEndpoint(string path) => OqtStartupHelper.IsSxcEndpoint(path);
    private static bool IsSxcDialog(string path) => OqtStartupHelper.IsSxcDialog(path);

    [Theory]
    [InlineData("app/testApp/api/testController/testAction")]
    [InlineData("app/testApp/edition/api/testController/testAction")]
    [InlineData("path/app/testApp/api/testController/testAction")]
    [InlineData("path/app/testApp/edition/api/testController/testAction")]
    [InlineData("path/subpath/app/testApp/api/testController/testAction")]
    [InlineData("path/subpath/app/testApp/edition/api/testController/testAction")]
    [InlineData("subsite/path/subpath/app/testApp/api/testController/testAction")]
    [InlineData("subsite/path/subpath/app/testApp/edition/api/testController/testAction")]
    public void IsSxcEndpoint_ShouldReturnTrue_ForValidPath(string validPath)
        => True(IsSxcEndpoint(validPath));

    [Theory]
    [InlineData("invalid/path/to/api")]
    [InlineData("app/testApp/api")]
    [InlineData("path/app/testApp/api/")]
    [InlineData("path/app/api/testController/testAction")]
    [InlineData("path/app//api/testController/testAction")]
    [InlineData("testApp/api/testController/testAction")]
    [InlineData("app/testApp/not-api/testController/testAction")]
    [InlineData("app/testApp/edition/not-api/testController/testAction")]
    [InlineData("path/app/testApp//api/testController/testAction")]
    [InlineData("path/app/testApp/not-api/testController/testAction")]
    [InlineData("path/app/testApp/edition/not-api/testController/testAction")]
    [InlineData("path/subpath/app/testApp/not-api/testController/testAction")]
    [InlineData("path/subpath/app/testApp/edition/not-api/testController/testAction")]
    [InlineData("subsite/path/subpath/app/testApp/not-api/testController/testAction")]
    [InlineData("subsite/path/subpath/app/testApp/edition/not-api/testController/testAction")]
    [InlineData("not-only-app/testApp/api")]
    public void IsSxcEndpoint_ShouldReturnFalse_ForInvalidPath(string invalidPath)
        => False(IsSxcEndpoint(invalidPath));

    [Theory]
    [InlineData("/Modules/" + OqtConstants.PackageName + "/dist/quick-dialog/")]
    [InlineData("/Modules/" + OqtConstants.PackageName + "/dist/ng-edit/")]
    public void IsSxcDialog_ShouldReturnTrue_ForValidPath(string validPath)
        => True(IsSxcDialog(validPath));

    [Theory]
    [InlineData("/invalid/path/to/dialog")]
    [InlineData("/Modules/InvalidPackageName/dist/quick-dialog/")]
    [InlineData("/Modules/InvalidPackageName/dist/ng-edit/")]
    public void IsSxcDialog_ShouldReturnFalse_ForInvalidPath(string invalidPath)
        => False(IsSxcDialog(invalidPath));
}