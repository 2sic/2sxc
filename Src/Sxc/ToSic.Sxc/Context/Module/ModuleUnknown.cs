using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Context
{
    public class ModuleUnknown: IModule, IIsUnknown
    {
        public IModule Init(int id, ILog parentLog)
        {
            // don't do anything
            return this;
        }

        public int Id => Eav.Constants.NullId;
        public bool IsPrimary => true;

        public IBlockIdentifier BlockIdentifier =>
            new BlockIdentifier(Eav.Constants.NullId, Eav.Constants.NullId, Guid.Empty, Guid.Empty);
    }
}
