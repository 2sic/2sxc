using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.LookUp.Sys;
using LookUpConstants = ToSic.Sxc.LookUp.Sys.LookUpConstants;

namespace ToSic.Sxc.DataSources;

public sealed partial class CmsBlock
{
    /// <summary>
    /// The block for which this DataSource is needed - provides context and configuration
    /// </summary>
    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal IBlock Block
    {
        get
        {
            if (field != null)
                return field;

            if (!Configuration.LookUpEngine.HasSource(LookUpConstants.InstanceContext))
                throw new("value provider didn't have sxc provider - can't use module data source");

            var instanceProvider = Configuration.LookUpEngine.FindSource(LookUpConstants.InstanceContext) as LookUpCmsBlock;
            field = instanceProvider?.Block
                    ?? throw new("value provider didn't have sxc provider - can't use module data source");

            return field;
        }
    }

    internal bool UseSxcInstanceContentGroup = false;
}