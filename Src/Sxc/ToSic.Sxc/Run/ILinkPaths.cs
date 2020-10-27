namespace ToSic.Sxc.Run
{
    public interface ILinkPaths
    {
        //string ToAppRoot(int appId, string appFolder);

        string ToAbsolute(string virtualPath);

        string ToAbsolute(string virtualPath, string subPath);
    }
}
