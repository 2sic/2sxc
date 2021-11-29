using ToSic.Eav.Documentation;
#if NETFRAMEWORK
using IHtmlString = System.Web.IHtmlString;
#else
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Implementation of IHtmlString in both .net Framework and .net Standard
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Helper to ensure that code providing an IHtmlString will work on .net Framework and .net Standard")]
    public interface IHybridHtmlString: IHtmlString
    {

    }
}
