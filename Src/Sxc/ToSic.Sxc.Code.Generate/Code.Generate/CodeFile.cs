using System.Collections.Generic;

namespace ToSic.Sxc.Code.Generate;

public class CodeFile: ICodeFile
{
    /// <inheritdoc />
    public string FileName { get; init; }
    /// <inheritdoc />
    public string Path { get; init; }
    /// <inheritdoc />
    public string Body { get; init; }

    /// <inheritdoc />
    public IReadOnlyCollection<ICodeFileInfo> Dependencies { get; init; } = [];
}