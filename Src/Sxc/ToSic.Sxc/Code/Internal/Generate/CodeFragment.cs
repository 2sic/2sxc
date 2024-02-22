﻿namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Temporary object to hold the generated code and usings,
/// so that the caller can later merge all required usings.
/// </summary>
/// <param name="code"></param>
/// <param name="usings"></param>
internal class CodeFragment(string nameId, string code, bool priority = true, List<string> usings = default, string closing = default)
{
    public string NameId { get; } = nameId;
    public bool Priority { get; } = priority;
    public List<string> Usings { get; } = usings ?? [];

    public override string ToString() => ToString(null);

    public string ToString(string contents) => code + contents + closing;
}