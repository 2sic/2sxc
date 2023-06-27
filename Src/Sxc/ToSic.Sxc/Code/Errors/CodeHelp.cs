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

        public CodeHelp(string name, string detect, string linkCode, string uiMessage = default, string detailsHtml = default)
        {
            DetailsHtml = detailsHtml;
            Name = name;
            Detect = detect;
            UiMessage = uiMessage;
            LinkCode = linkCode;
        }
        /// <summary>
        /// Name for internal use to better understand what this is for
        /// </summary>
        public string Name { get; }
        public string Detect { get; }
        public string UiMessage { get; }
        public string DetailsHtml { get; }

        public readonly string LinkCode;
        public string Link => !LinkCode.HasValue() ? "" : LinkCode.Contains("http") ? LinkCode : string.Format(ErrHelpLink, LinkCode);

        public string LinkMessage => LinkCode.HasValue() ? string.Format(ErrLinkMessage, Link) : "";

        public string ErrorMessage => $"{ErrHelpPre} {UiMessage} {LinkMessage} {(DetailsHtml != null ? ErrHasDetails : "")} {ErrHelpSuf}";
    }
}
