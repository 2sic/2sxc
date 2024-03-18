using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Data which is meant to create a new file.
/// The file will usually be saved directly to the file system of the App.
///
/// This is WIP - goal is to make it more standard so others could create generators too.
/// </summary>
/// <remarks>
/// Introduced in v17.05
/// </remarks>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICodeFile : ICodeFileInfo
{
    /// <summary>
    /// The body of the file, which will be written to the file.
    /// </summary>
    string Body { get; }

    /// <summary>
    /// List of dependencies which are required to create this file.
    ///
    /// For example, if we generate a PersonList.cshtml which will inherit the AppRazor, then the AppRazor should be in the list.
    ///
    /// Note: not implemented yet - this is for later when users may want to select which files to generate.
    /// </summary>
    IReadOnlyCollection<ICodeFileInfo> Dependencies { get; }
}