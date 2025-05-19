namespace ToSic.Sxc.Adam.Work.Internal;

public record AdamFolderFileSet(
    IFolder Root,
    IList<IFolder> Folders,
    IList<IFile> Files
);
