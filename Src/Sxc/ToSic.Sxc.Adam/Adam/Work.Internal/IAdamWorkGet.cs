namespace ToSic.Sxc.Adam.Work.Internal;

public interface IAdamWorkGet: IAdamWork
{
    AdamFolderFileSet ItemsInField(string subFolderName, bool autoCreate = false);
}