using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Errors
{
    public class CodeError
    {
        public const string ErrHelpPre = "Error in your code. ";
        public const string ErrHelpLink = "https://go.2sxc.org/{0}";
        public const string ErrLinkMessage = "***** Probably {0} can help! ***** \n";
        public const string ErrHelpSuf = "What follows is the internal error: ";

        public CodeError(string name, string detect, string linkCode, string uiMessage = null)
        {
            Name = name;
            Detect = detect;
            UiMessage = uiMessage;
            LinkCode = linkCode;
        }
        /// <summary>
        /// Name for internal use to better understand what this is for
        /// </summary>
        public readonly string Name;
        public readonly string Detect;
        public readonly string UiMessage;
        public readonly string LinkCode;
        public string Link => !LinkCode.HasValue() ? "" : LinkCode.Contains("http") ? LinkCode : string.Format(ErrHelpLink, LinkCode);

        public string LinkMessage => LinkCode.HasValue() ? string.Format(ErrLinkMessage, Link) : "";

        public string ErrorMessage => $"{ErrHelpPre} {UiMessage} {LinkMessage} {ErrHelpSuf}";
    }
}
