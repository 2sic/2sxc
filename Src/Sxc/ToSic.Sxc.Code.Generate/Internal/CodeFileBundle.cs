using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// WIP - should contain a list of code-files and additional specs
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeFileBundle : ICodeFileBundle
{
    /// <summary>
    /// Name for easy identification.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description to show in the UI
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Name of the generator, possibly with version
    /// </summary>
    public string Generator { get; set; }

    /// <summary>
    /// The path, but not sure yet how to do, especially if it will be relative or contain editions.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// List of files to generate
    /// </summary>
    public List<ICodeFile> Files { get; set; }
}