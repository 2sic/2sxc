using ToSic.Lib.Documentation;
#if NETFRAMEWORK
using IHtmlString = System.Web.IHtmlString;
#else
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Implementation of IHtmlString in both .net Framework and .net Standard.
    ///
    /// This means that any such object will automatically be *Raw* output if used as `@SomeHybridHtmlString` so it's the same as `Html.Raw(normalString)`
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Helper to ensure that code providing an IHtmlString will work on .net Framework and .net Standard")]
    public interface IHybridHtmlString: IHtmlString
    {

    }
}
