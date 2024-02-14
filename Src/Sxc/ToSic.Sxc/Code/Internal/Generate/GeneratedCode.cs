using System.Text;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Temporary object to hold the generated code and usings,
/// so that the caller can later merge all required usings.
/// </summary>
/// <param name="builder"></param>
/// <param name="usings"></param>
internal class GeneratedCode(StringBuilder builder, List<string> usings = default)
{
    public StringBuilder Builder { get; } = builder;
    public List<string> Usings { get; } = usings ?? [];
}