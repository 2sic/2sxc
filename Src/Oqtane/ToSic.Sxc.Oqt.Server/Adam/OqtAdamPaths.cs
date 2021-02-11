using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    /// <summary>
    /// Basic AdamPaths resolver, assumes that files are in Content/[tenant]/site/[site]/adam for now
    /// </summary>
    public class OqtAdamPaths: AdamPathsBase
    {
        public OqtAdamPaths(IServerPaths serverPaths) : base(serverPaths, LogNames.Basic)
        {
        }

        public override string Url(string path)
        {
            var original = base.Url(path);
            return original.PrefixSlash();
        }
    }
}
