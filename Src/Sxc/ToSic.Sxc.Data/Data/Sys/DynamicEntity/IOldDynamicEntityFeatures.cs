using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys;


public interface IOldDynamicEntityFeatures
{
    System.Web.IHtmlString GenerateOldToolbar(ICodeDataFactory cdf, IEntity entity);
    IRawHtmlString Render(ICodeDataFactory cdf, ICanBeItem target);
}