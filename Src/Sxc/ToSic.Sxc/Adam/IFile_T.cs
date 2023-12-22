namespace ToSic.Sxc.Adam;

/// <summary>
/// An ADAM (Automatic Digital Asset Management) file
/// </summary>
public interface IFile<out TFolderId, out TFileId>: 
    IAsset, 
    Eav.Apps.Assets.IFile<TFolderId, TFileId>
{
}