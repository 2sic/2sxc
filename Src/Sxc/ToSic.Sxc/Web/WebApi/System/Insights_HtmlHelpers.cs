using System;
using System.Linq;
using System.Text;
using System.Web;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        private static ITag HeadFields(params object[] fields)
            => Tag.Thead(Tag.Tr(
                fields.Select(fresh => Tag.Th(HtmlEncode((fresh ?? "").ToString()))).ToArray<object>()
            ));

        private static ITag RowFields(params object[] fields)
            => Tag.Tr(fields.Select(fresh => Tag.Td((fresh ?? "").ToString())).ToArray<object>());

        private const string JsTableSortCdn = "https://cdnjs.cloudflare.com/ajax/libs/tablesort/5.0.2/";

        private static TagBase JsTableSort(string id = "table")
            => Tag.Script().Src(JsTableSortCdn + "tablesort.min.js")
               + Tag.Script().Src(JsTableSortCdn + "sorts/tablesort.number.min.js")
               + Tag.Script($"new Tablesort(document.getElementById('{id}'));");




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
            => Tag.Span(label).Class(classes).Title(text).ToString();

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

        private static string EmojiTrueFalse(bool value) => HtmlEncode(value ? "✅" : "⛔");

    }
}
