namespace ToSic.Sxc.Adam.Sys.Work;

/// <summary>
/// Bundle of files/folders describing a folder and things inside it.
/// </summary>
/// <param name="Root">the root folder, which must usually be treated in a special way when converting to DTO</param>
/// <param name="Folders">All folders inside the root.</param>
/// <param name="Files">All files inside the root.</param>
public record AdamFolderFileSet(
    IFolder Root,
    IList<IFolder> Folders,
    IList<IFile> Files
);
