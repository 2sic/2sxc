namespace ToSic.SexyContent.WebApi
{
    public partial class DebugController
    {
        private static string Tag(string tag, string content, string attributes = null) => $"<{tag}{attributes}>{content}</{tag}>";
        private static string Attribute(string attribute, string content) => $" {attribute}={content} ";
        // ReSharper disable InconsistentNaming
        private static string h1(string content) => Tag("h1", content);
        private static string li(string content) => Tag("li", content);
        private static string a(string content, string link) => Tag("a", content, Attribute("href", link));

        private static string p(string content) => Tag("p", content);
        // ReSharper restore InconsistentNaming

        private static string ToBr(string html) => html.Replace("\n", "<br>\n");
    }
}