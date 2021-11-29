using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    /// <inheritdoc />
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Helper to ensure that code providing an IHtmlString will work on .net Framework and .net Standard")]
    public class HybridHtmlString: IHybridHtmlString
    {
        public HybridHtmlString(string value)
        {
            Value = value;
        }

        protected string Value { get; }

        /// <summary>
        /// Standard ToString overload - used when concatenating strings.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value;

#if NETSTANDARD
        public void WriteTo(System.IO.TextWriter writer, System.Text.Encodings.Web.HtmlEncoder encoder) 
            => new Microsoft.AspNetCore.Html.HtmlString(Value).WriteTo(writer, encoder);
#endif

#if NETFRAMEWORK
        public string ToHtmlString() => Value;
#endif
    }
}
