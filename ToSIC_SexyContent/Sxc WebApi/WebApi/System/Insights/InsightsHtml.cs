using System;
using System.Text;
using System.Web;

namespace ToSic.Sxc.WebApi.System
{
    internal class InsightsHtml
    {
        internal static string PageStyles() =>
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

ol {
    padding-inline-start: 20px;
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

        internal static string HoverLabel(string label, string text, string classes)
            => $"<span class='{classes}' title='{text}'>{label}</span>";

        internal static string HtmlEncode(string text)
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
