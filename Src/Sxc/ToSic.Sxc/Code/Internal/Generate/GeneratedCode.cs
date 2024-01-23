using System.Text;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratedCode(StringBuilder builder, List<string> usings = default)
{
    public StringBuilder Builder { get; } = builder;
    public List<string> Usings { get; } = usings ?? [];
}