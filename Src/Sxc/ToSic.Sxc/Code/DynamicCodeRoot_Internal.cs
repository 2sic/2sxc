using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        [PrivateApi]
        public void LateAttachApp(IApp app)
        {
            App = app;
        }

        [PrivateApi]
        public int CompatibilityLevel { get; private set; }

        [PrivateApi] public IBlock Block { get; private set; }

    }
}
