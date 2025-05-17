using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Adam.Work.Internal;

public record AdamFolderFileSet<TFolder, TFile>(
    Folder<TFolder, TFile> Root,
    IList<Folder<TFolder, TFile>> Folders,
    IList<File<TFolder, TFile>> Files);
