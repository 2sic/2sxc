using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.LookUp.Sys;
using LookUpConstants = ToSic.Sxc.LookUp.Sys.LookUpConstants;

namespace ToSic.Sxc.DataSources;

internal class CmsBlockInstanceFinder
{
    internal IBlock GetFromConfiguration(IDataSourceConfiguration configuration)
    {
        if (!configuration.LookUpEngine.HasSource(LookUpConstants.InstanceContext))
            throw new("value provider didn't have sxc provider - can't use module data source");

        var instanceProvider = configuration.LookUpEngine.FindSource(LookUpConstants.InstanceContext) as LookUpCmsBlock;
        return instanceProvider?.Block
               ?? throw new("value provider didn't have sxc provider - can't use module data source");

    }

}