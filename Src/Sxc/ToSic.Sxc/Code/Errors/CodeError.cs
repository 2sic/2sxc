using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Errors
{
    internal class CodeError
    {
        public const string ErrHelpPre = "Error in your code. ";
        public const string ErrHelpLink = "***** Probably https://go.2sxc.org/{0} can help! ***** \n";
        public const string ErrHelpSuf = "What follows is the internal error: ";

        public CodeError(string name, string detect, string linkCode, string message = null)
        {
            Name = name;
            Detect = detect;
            _innerMessage = message;
            LinkCode = linkCode;
        }
        /// <summary>
        /// Name for internal use to better understand what this is for
        /// </summary>
        public readonly string Name;
        public readonly string Detect;
        private readonly string _innerMessage;
        public readonly string LinkCode;
        private string Link => LinkCode.HasValue() ? string.Format(ErrHelpLink, LinkCode) : "";

        public string Message => $"{ErrHelpPre} {_innerMessage} {Link} {ErrHelpSuf}";
    }
}
