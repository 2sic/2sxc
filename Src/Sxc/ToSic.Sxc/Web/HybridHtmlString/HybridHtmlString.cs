using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web
{
    /// <inheritdoc />
    [PrivateApi("Helper to ensure that code providing an IHtmlString will work on .net Framework and .net Standard")]
    public class HybridHtmlString: TagText, IHybridHtmlString, IString
    {
        public HybridHtmlString(string value) : base(value) { }
            // => _value = value;

        protected HybridHtmlString() : base("") { }
            //=> _value = "";
        //private readonly string _value;

//        /// <summary>
//        /// Standard ToString overload - used when concatenating strings.
//        /// </summary>
//        /// <returns></returns>
//        public override string ToString() => _value;

//#if NETFRAMEWORK
//        public string ToHtmlString() => ToString();
//#else
//        public void WriteTo(System.IO.TextWriter writer, System.Text.Encodings.Web.HtmlEncoder encoder) 
//            => new Microsoft.AspNetCore.Html.HtmlString(ToString()).WriteTo(writer, encoder);
//#endif
    }
}
