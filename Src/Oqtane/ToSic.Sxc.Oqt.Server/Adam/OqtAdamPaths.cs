using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    /// <summary>
    /// Basic AdamPaths resolver, assumes that files are in Content/[tenant]/site/[site]/adam for now
    /// </summary>
    public class OqtAdamPaths: AdamPathsBase
    {
        private readonly SiteStateInitializer _siteStateInitializer;

        public OqtAdamPaths(IServerPaths serverPaths, SiteStateInitializer siteStateInitializer) : base(serverPaths, LogNames.Basic)
        {
            _siteStateInitializer = siteStateInitializer;
        }

        public string Path(string path)
        {
            var original = base.Url(path);
            return original.PrefixSlash();
        }

        public override string Url(string path)
        {
            // Convert path to url.
            string url = path.ForwardSlash();
            var parts = url.Split("/").ToList();
            if (parts[0] == "adam")
            {
                // Swap 'adam' and appName.
                parts[0] = parts[1];
                parts[1] = "adam";

                // Insert alias path.
                //_siteStateInitializer.InitIfEmpty();
                var alias = _siteStateInitializer.InitializedState.Alias;// _siteStateInitializer.SiteState.Alias;
                var aliasPath = alias.Path;
                parts.Insert(0, $"{aliasPath}/app");

                // Build url string.
                url = string.Join('/', parts.ToArray());
            }

            return url.PrefixSlash();
        }
    }
}
