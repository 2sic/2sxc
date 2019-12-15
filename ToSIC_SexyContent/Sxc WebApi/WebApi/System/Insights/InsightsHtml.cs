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

    }
}
