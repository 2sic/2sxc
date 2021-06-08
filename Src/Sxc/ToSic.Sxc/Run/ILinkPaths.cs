namespace ToSic.Sxc.Run
{
    public interface ILinkPaths
    {
        //string ToAppRoot(int appId, string appFolder);

        string AsSeenFromTheDomainRoot(string virtualPath);

        //string ToAbsolute(string virtualPath, string subPath);

        //string AppAsset(string virtualPath);

        //string AppAssetsBase(ISite site, IApp app);
        
        // string AbsoluteToAppRootWip()
    }
}
