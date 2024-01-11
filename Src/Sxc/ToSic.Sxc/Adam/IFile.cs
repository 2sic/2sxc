namespace ToSic.Sxc.Adam;

/// <summary>
/// An ADAM (Automatic Digital Asset Management) file
/// This simple interface assumes that it uses int-IDs.
/// </summary>
[PublicApi]

public interface IFile: 
    IAsset,
    Eav.Apps.Assets.IFile
{
}