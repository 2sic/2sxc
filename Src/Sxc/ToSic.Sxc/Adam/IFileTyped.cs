using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// An ADAM (Automatic Digital Asset Management) file
    /// This simple interface assumes that it uses int-IDs.
    /// </summary>
    [WorkInProgressApi("Still WIP v16.02")]

    public interface IFileTyped :
        IAssetTyped,
        IFile
    {
    }
}
