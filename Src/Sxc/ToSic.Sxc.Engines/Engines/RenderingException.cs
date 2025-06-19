using ToSic.Sys.Code.Help;
using ToSic.Sys.Exceptions;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class RenderingException: Exception, IExceptionWithHelp
{
    public RenderingException(CodeHelp help, string? message = default) : base(message ?? help.UiMessage)
    {
        Helps = [help];
    }
        

    public RenderingException(CodeHelp help, Exception inner) : base("Rendering Exception", inner)
    {
        Helps = [help];
    }

    public List<CodeHelp> Helps { get; }
}