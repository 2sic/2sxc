using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Bundle of code files to generate, including some information about the generator.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICodeFileBundle
{
    /// <summary>
    /// Name for easy identification.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description to show in the UI
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Name of the generator, possibly with version
    /// </summary>
    string Generator { get; }

    /// <summary>
    /// The path, but not sure yet how to do, especially if it will be relative or contain editions.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// List of files to generate
    /// </summary>
    IReadOnlyCollection<ICodeFile> Files { get; }
}