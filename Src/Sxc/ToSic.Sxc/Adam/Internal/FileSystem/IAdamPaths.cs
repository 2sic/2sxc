namespace ToSic.Sxc.Adam.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IAdamPaths: IHasLog
{
    IAdamPaths Init(AdamManager adamManager);

    string PhysicalPath(string path);

    string RelativeFromAdam(string path);

    string Url(string path);
}