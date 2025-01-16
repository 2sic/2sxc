using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.LookUp;
using LookUpConstants = ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.DataSources;

public sealed partial class CmsBlock
{
    /// <summary>
    /// The block for which this DataSource is needed - provides context and configuration
    /// </summary>
    [PrivateApi]
    internal IBlock Block
    {
        get
        {
            if (_block != null) return _block;

            if (!Configuration.LookUpEngine.HasSource(LookUpConstants.InstanceContext))
                throw new("value provider didn't have sxc provider - can't use module data source");

            var instanceProvider = Configuration.LookUpEngine.FindSource(LookUpConstants.InstanceContext) as LookUpCmsBlock;
            _block = instanceProvider?.Block
                     ?? throw new("value provider didn't have sxc provider - can't use module data source");

            return _block;
        }
    }
    private IBlock _block;

    internal bool UseSxcInstanceContentGroup = false;
}