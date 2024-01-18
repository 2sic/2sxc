#if NETFRAMEWORK
using ToSic.Sxc.Adam;

// Obsolete class / namespace
// Used in some previous apps like BlueImp
// Leave for compatibility

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Adam;

// ReSharper disable once InconsistentNaming
[Obsolete("use ToSic.Sxc.Adam.IFile instead")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface AdamFile: IFile
{
    [Obsolete("use FullName instead")]
    string FileName { get; }

    [Obsolete("use Created instead")]
    DateTime CreatedOnDate { get; }

    [Obsolete("use Id instead")]
    int FileId { get; }

}
#endif