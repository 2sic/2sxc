using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// An ADAM (Automatic Digital Asset Management) file
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]

    public interface IFile: 
        IAsset, 
        Eav.Apps.Assets.IFile
    {
    }
}
