using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// WIP - should contain a list of code-files and additional specs
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeFileBundle : ICodeFileBundle
{
    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public string Description { get; init; }

    /// <inheritdoc />
    public string Generator { get; init; }

    /// <inheritdoc />
    public string Path { get; init; }

    /// <inheritdoc />
    public IReadOnlyCollection<ICodeFile> Files { get; init; }
}