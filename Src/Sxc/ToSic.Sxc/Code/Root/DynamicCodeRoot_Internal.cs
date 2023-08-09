using System;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        [PrivateApi]
        public void AttachApp(IApp app)
        {
            if (app is App typedApp) typedApp.SetupAsConverter(Cdf);
            App = app;
        }

        [PrivateApi]
        [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
        public int CompatibilityLevel => Cdf.CompatibilityLevel;

        [PrivateApi] public IBlock Block { get; private set; }

    }
}
