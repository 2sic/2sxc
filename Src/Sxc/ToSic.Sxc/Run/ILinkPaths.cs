namespace ToSic.Sxc.Run
{
    public interface ILinkPaths
    {
        string ToAbsolute(string virtualPath);

        string ToAbsolute(string virtualPath, string subPath);
    }
}
