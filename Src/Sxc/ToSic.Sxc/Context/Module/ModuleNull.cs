using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Context
{
    public class ModuleUnknown: IModule
    {
        public IModule Init(int id, ILog parentLog)
        {
            // don't do anything
            return this;
        }

        public int Id => Eav.Constants.NullId;
        public bool IsPrimary => true;
        public IBlockIdentifier BlockIdentifier => null;
    }
}
