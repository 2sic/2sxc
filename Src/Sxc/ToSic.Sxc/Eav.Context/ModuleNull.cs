using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    public class ModuleNull: IModule
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
