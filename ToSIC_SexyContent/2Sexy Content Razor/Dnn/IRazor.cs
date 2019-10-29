using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// All DNN Razor Pages inherit from this class
    /// </summary>
    [PublicApi]
    public interface IRazor: IDynamicCode
    {
        /// <summary>
        /// Helper for Html.Raw - for creating raw html output which doesn't encode &gt; and &lt;
        /// </summary>
        IHtmlHelper Html { get; }


    }
}
