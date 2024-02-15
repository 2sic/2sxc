namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Temporary object to hold the generated code and usings,
/// so that the caller can later merge all required usings.
/// </summary>
/// <param name="code"></param>
/// <param name="usings"></param>
internal class GenCodeSnippet(string nameId, string code, bool priority = true, List<string> usings = default)
{
    public string NameId { get; } = nameId;
    public string Code { get; } = code;
    public bool Priority { get; } = priority;
    public List<string> Usings { get; } = usings ?? [];
}