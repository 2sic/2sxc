using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Errors
{
    public class CodeHelp
    {
        public const string ErrHelpPre = "Error in your code. ";
        private const string ErrHelpLink = "https://go.2sxc.org/{0}";
        private const string ErrLinkMessage = "***** Probably {0} can help! ***** \n";
        private const string ErrHasDetails = "***** You can see more help in the toolbar. ***** \n ";
        private const string ErrHelpSuf = "What follows is the internal error: -------------------------";

        public CodeHelp(string name, string detect, string linkCode = default, bool detectRegex = default, string uiMessage = default, string detailsHtml = default)
            : this(original: null, name: name, detect: detect, linkCode: linkCode, detectRegex: detectRegex, uiMessage: uiMessage, detailsHtml: detailsHtml)
        {}

        public CodeHelp(CodeHelp original, string name = default, string detect = default, string linkCode = default, bool? detectRegex = default, string uiMessage = default, string detailsHtml = default)
        {
            Name = name ?? original?.Name;
            LinkCode = linkCode ?? original?.LinkCode;
            Detect = detect ?? original?.Detect;
            DetectRegex = detectRegex ?? original?.DetectRegex ?? false;
            UiMessage = uiMessage ?? original?.UiMessage;
            DetailsHtml = detailsHtml ?? original?.DetailsHtml;
        }
        /// <summary>
        /// Name for internal use to better understand what this is for
        /// </summary>
        public string Name { get; }
        public string Detect { get; }
        public bool DetectRegex { get; }
        public string UiMessage { get; }
        public string DetailsHtml { get; }

        public readonly string LinkCode;
        public string Link => !LinkCode.HasValue() ? "" : LinkCode.Contains("http") ? LinkCode : string.Format(ErrHelpLink, LinkCode);

        public string LinkMessage => LinkCode.HasValue() ? string.Format(ErrLinkMessage, Link) : "";

        public string ErrorMessage => $"{ErrHelpPre} {UiMessage} {LinkMessage} {(DetailsHtml != null ? ErrHasDetails : "")} {ErrHelpSuf}";
    }
}
