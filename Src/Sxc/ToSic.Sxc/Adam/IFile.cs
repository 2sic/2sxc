using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Adam;

/// <summary>
/// An ADAM (Automatic Digital Asset Management) file
/// This simple interface assumes that it uses int-IDs.
/// </summary>
[PublicApi_Stable_ForUseInYourCode]

public interface IFile: 
    IAsset,
    Eav.Apps.Assets.IFile
{
}