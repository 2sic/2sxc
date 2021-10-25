using ToSic.Sxc.Dnn.Web;

namespace ToSic.Sxc.Dnn
{
    public interface IDnnRazor
    {
        /// <summary>
        /// Helper for Html.Raw - for creating raw html output which doesn't encode &gt; and &lt;
        /// </summary>
        IHtmlHelper Html { get; }

    }
}
