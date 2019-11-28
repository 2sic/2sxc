using System.Linq;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {
        private static string Tag(string tag, string content, string attributes = null) => $"<{tag}{attributes}>{content}</{tag}>";
        private static string Atr(string attribute, string content) => $" {attribute}={content} ";
        // ReSharper disable InconsistentNaming
        private static string h1(string content) => Tag("h1", content);
        private static string li(string content) => Tag("li", content);
        private static string a(string content, string link, bool newWindow = false) => Tag("a", content, Atr("href", link) + (newWindow ? Atr("target", "_blank"):""));

        private static string p(string content) => Tag("p", content);

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
    }
}