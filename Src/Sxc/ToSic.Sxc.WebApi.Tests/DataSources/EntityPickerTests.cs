using ToSic.Eav.Data;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Entities;
using Xunit;

namespace ToSic.Sxc.WebApi.Tests.DataSources;

public class EntityPickerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetList_ReturnsPublishedAndDraftEntities_ForModuleEditor(bool filterByType)
    {
        using var ctx = new EntityPickerTestContext();
        var picker = ctx.CreatePicker(ctx.BlockContext(), filterByType);

        var items = picker.List.ToList();

        Assert.Equal(ctx.EntityIds, items.Select(e => e.EntityId).OrderBy(id => id));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetList_DeniesAccess_WithoutModuleEditPermission(bool filterByType)
    {
        using var ctx = new EntityPickerTestContext();
        var picker = ctx.CreatePicker(ctx.BlockContext(canEdit: false), filterByType);

        AssertPermissionDenied(picker.List);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetList_DeniesAccess_WithoutPreparedContext(bool filterByType)
    {
        using var ctx = new EntityPickerTestContext();
        var picker = ctx.CreatePicker(null, filterByType);

        AssertPermissionDenied(picker.List);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetList_DeniesAccess_WithAppOnlyContext(bool filterByType)
    {
        using var ctx = new EntityPickerTestContext();
        var picker = ctx.CreatePicker(ctx.AppContext(), filterByType);

        AssertPermissionDenied(picker.List);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetList_DeniesAccess_WhenNestedQueryTargetsAnotherApp(bool filterByType)
    {
        using var ctx = new EntityPickerTestContext();
        var picker = ctx.CreatePicker(ctx.BlockContext(otherApp: true), filterByType);

        AssertPermissionDenied(picker.List);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetList_ReturnsEntities_ForAdministratorWithoutModule(bool filterByType)
    {
        using var ctx = new EntityPickerTestContext(isSystemAdmin: true);
        var picker = ctx.CreatePicker(null, filterByType);

        Assert.Equal(ctx.EntityIds, picker.List.Select(e => e.EntityId).OrderBy(id => id));
    }

    private static void AssertPermissionDenied(IEnumerable<IEntity> items)
    {
        var error = Assert.Single(items);
        Assert.Equal(DataConstants.ErrorTypeName, error.Type.Name);
        Assert.Contains("No permissions", error.GetBestTitle());
    }
}
