using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Block
{
    [PrivateApi]
    public class OqtaneDynamicCodeRoot : DynamicCodeRoot
    {
        private readonly Lazy<SiteStateInitializer> _siteStateInitializerLazy;
        public OqtaneDynamicCodeRoot(Dependencies dependencies, Lazy<SiteStateInitializer> siteStateInitializerLazy) : base(dependencies, OqtConstants.OqtLogPrefix)
        {
            _siteStateInitializerLazy = siteStateInitializerLazy;
        }

        public override IDynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility = 10)
        {
            _siteStateInitializerLazy.Value.InitIfEmpty(block?.Context?.Site?.Id);
            return base.Init(block, parentLog, compatibility);
        }
    }
}
