using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys;


[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOldDynamicEntityFeatures
{
    System.Web.IHtmlString GenerateOldToolbar(ICodeDataFactory cdf, IEntity entity);
    IRawHtmlString Render(ICodeDataFactory cdf, ICanBeItem target);
}