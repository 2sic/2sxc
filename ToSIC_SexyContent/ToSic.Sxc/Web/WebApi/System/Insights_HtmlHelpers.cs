using System;
using System.Linq;
using System.Text;
using System.Web;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        private static string Tag(string tag, string content, string attributes = null) => $"<{tag}{attributes}>{content}</{tag}>";
        private static string Atr(string attribute, string content) => $" {attribute}={content} ";
        // ReSharper disable InconsistentNaming
        private static string h1(string content) => Tag("h1", content);
        private static string h2(string content) => Tag("h2", content);

        private static string li(string content) => Tag("li", content);
        private static string a(string content, string link, bool newWindow = false) => Tag("a", content, Atr("href", link) + (newWindow ? Atr("target", "_blank") : ""));

        private static string p(string content) => Tag("p", content);

        private static string div(string content) => Tag("div", content);

        private static string em(string content) => Tag("em", content);
        private static string strong(string content) => Tag("strong", content);

        private static string summary(string title, string content) => Tag("details", Tag("summary", title) + content);

        private static string td(string content) => Tag("td", content);
        private static string tr(string[] content, bool head = false)
            => Tag("tr", content.Aggregate("", (all, c) => all + Tag(head ? "th" : "td", c)));


        // ReSharper restore InconsistentNaming

        private static string ToBr(string html) => html.Replace("\n", "<br>\n");

        private const string JsTableSortCdn = "https://cdnjs.cloudflare.com/ajax/libs/tablesort/5.0.2/";

        private static string JsTableSort(string id = "table")
            => Tag("script", "", Atr("src", JsTableSortCdn + "tablesort.min.js"))
               + Tag("script", "", Atr("src", JsTableSortCdn + "sorts/tablesort.number.min.js"))
               + Tag("script", $"new Tablesort(document.getElementById('{id}'));");




        private static string PageStyles() =>
            @"
<style>
.logIds {
    color: darkgray;
}

.codePeek {
    color: blue;
}

.result {
    color: green;
}
/* first ol level needs more padding, because it can number up to 4 digits */
body>ol {
    padding-inline-start: 40px;
}

/* all other OL levels need smaller padding, as they should be aligned nicely */
ol {
    padding-inline-start: 23px;
    list-style: none; 
    counter-reset: li;
}
ol li::before {
    counter-increment: li;
    content: '.'counter(li); 
    /* color: red; */
    display: inline-block; 
    width: 1em; margin-left: -1.5em;
    margin-right: 0.5em; 
    text-align: right; 
    direction: rtl;
}

ol li ol li:: before {
    color: blue;
}
</style>";

        private static string HoverLabel(string label, string text, string classes)
            => $"<span class='{classes}' title='{text}'>{label}</span>";

        private static string HtmlEncode(string text)
        {
            if (text == null) return "";
            var chars = HttpUtility.HtmlEncode(text).ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (var c in chars)
            {
                var value = Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

    }
}
