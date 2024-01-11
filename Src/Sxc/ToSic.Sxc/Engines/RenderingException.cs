using ToSic.Eav;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class RenderingException: Exception, IExceptionWithHelp
{
    public RenderingException(CodeHelp help, string message = default) : base(message ?? help.UiMessage)
    {
        Helps = [help];
    }
        

    public RenderingException(CodeHelp help, Exception inner) : base("Rendering Exception", inner)
    {
        Helps = [help];
    }

    public List<CodeHelp> Helps { get; }
}