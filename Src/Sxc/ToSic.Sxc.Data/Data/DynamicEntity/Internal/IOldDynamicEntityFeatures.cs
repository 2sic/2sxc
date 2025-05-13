using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data.Internal;


public interface IOldDynamicEntityFeatures
{
    System.Web.IHtmlString GenerateOldToolbar(ICodeDataFactory cdf, IEntity entity);
    IRawHtmlString Render(ICodeDataFactory cdf, ICanBeItem target);
}