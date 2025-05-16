using ToSic.Eav.Plumbing;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Internal;
using ToSic.Sxc.Edit.Toolbar.Internal;

namespace ToSic.Sxc.Edit.ToolbarConfigurationTests;

public class ToolbarConfigurationShowByRole
{
    [Theory]
    [InlineData("", "", null, null)]
    [InlineData("", null, null, null)]
    [InlineData("", "CarEditors", null, null)]
    [InlineData("", null, "CarEditors", null)]

    [InlineData("NotInList", null, null, null)]
    [InlineData("NotInList", "", null, null)]
    [InlineData("NotInList", "CarEditors", null, null)]

    [InlineData("NotInList", null, "", null)]
    [InlineData("NotInList", null, "CarEditors", null)]

    [InlineData("CarEditors", "CarEditors", null, true)]
    [InlineData("CarEditors", "carEditors", null, true)]
    [InlineData("CarEditors,Other", "CarEditors", null, true)]
    [InlineData("Other,CarEditors", "CarEditors", null, true)]
    [InlineData("CarEditors", "Abc,CarEditors", null, true)]
    [InlineData("CarEditors,Other", "Abc,CarEditors", null, true)]
    [InlineData("CarEditors", "Abc,CarEditors,More", null, true)]

    [InlineData("CarEditors", null, "CarEditors", false)]
    [InlineData("CarEditors", null, "carEditors", false)]
    [InlineData("CarEditors,Other", null, "CarEditors", false)]
    [InlineData("Other,CarEditors", null, "CarEditors", false)]
    [InlineData("CarEditors", null, "Abc,CarEditors", false)]
    [InlineData("CarEditors,Other", null, "Abc,CarEditors", false)]
    [InlineData("CarEditors", null, "Abc,CarEditors,More", false)]
    public void TestRoles(string userRoles, string? showFor, string? denyFor, bool? expected)
    {
        var config = new ToolbarBuilderConfiguration
        {
            ShowForRoles = showFor?.CsvToArrayWithoutEmpty().ToList(),
            ShowDenyRoles = denyFor?.CsvToArrayWithoutEmpty().ToList(),
        };

        var user = new UserModel
        {
            Roles = userRoles.CsvToArrayWithoutEmpty()
                .Select(IUserRoleModel (ur) => new UserRoleModel { Name = ur })
        };

        var show = new ToolbarConfigurationShowHelper().OverrideShowBecauseOfRoles(config, user);

        Equal(expected, show);
    }
}