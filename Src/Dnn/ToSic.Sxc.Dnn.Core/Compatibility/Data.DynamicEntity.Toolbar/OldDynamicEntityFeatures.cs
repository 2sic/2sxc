#if NETFRAMEWORK

using ToSic.Eav.Data;
using ToSic.Razor.Markup;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys;
using ToSic.Sxc.Data.Sys.CodeDataFactory;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Compatibility;

/// <summary>
/// Helper so that the old DynamicEntity can get a toolbar
/// </summary>
internal class OldDynamicEntityFeatures : IOldDynamicEntityFeatures
{
    public System.Web.IHtmlString GenerateOldToolbar(ICodeDataFactory cdf, IEntity entity)
    {
        // 2025-05-13 2dm old code, must ensure that this code doesn't need the IBlockContext
        //var userMayEdit = Cdf.BlockOrNull?.Context.Permissions.IsContentAdmin ?? false;

        // If we're not in a running context, of which we know the permissions, no toolbar
        // Internally also verifies that we have a context (otherwise it's false), so no toolbar
        var userMayEdit = ((CodeDataFactory)cdf).BlockOrNull?.Context.Permissions.IsContentAdmin ?? false;

        if (!userMayEdit)
            return new System.Web.HtmlString("");

        if (cdf.CompatibilityLevel > CompatibilityLevels.MaxLevelForEntityDotToolbar)
            throw new("content.Toolbar is deprecated in the new RazorComponent. Use @Edit.TagToolbar(content) or @Edit.Toolbar(content) instead. See https://go.2sxc.org/EditToolbar");

        var toolbar = new Edit.Toolbar.ItemToolbar(entity).ToolbarAsTag;
        return new System.Web.HtmlString(toolbar);

    }

    public IRawHtmlString Render(ICodeDataFactory cdf, ICanBeItem target)
    {
        if (cdf.CompatibilityLevel > CompatibilityLevels.MaxLevelForEntityDotRender)
            throw new("content.Render() is deprecated in the new RazorComponent. Use GetService&lt;ToSic.Sxc.Services.IRenderService&gt;().One(content) instead.");

        return cdf.GetService<IRenderService>().One(target);
    }
}
#endif
