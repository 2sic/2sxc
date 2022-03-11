using ToSic.Eav;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Basic AdamPaths resolver, assumes that files are in wwwroot/adam for now.
    /// </summary>
    public class AdamPathsWwwroot: AdamPathsBase
    {
        public AdamPathsWwwroot(IServerPaths serverPaths) : base(serverPaths, LogNames.Basic)
        {
        }

        /// <summary>
        /// This will just assume that the path - containing 'wwwroot' will not have the 'wwwroot' in the link from outside
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string Url(string path)
        {
            var original = base.Url(path);
            var url = "/" + original.Replace("wwwroot/", "");
            return url;
        }
    }
}
