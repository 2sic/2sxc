using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Features
{
    internal class FeaturesHelpers
    {
        internal static IEnumerable<Feature> FeatureListWithPermissionCheck(MultiPermissionsApp permCheck)
        {
            // if the user has full edit permissions, he may also get the un-public features
            // otherwise just the public Ui features
            var includeNonPublic = permCheck.UserMayOnAll(GrantSets.WritePublished);

            return Eav.Configuration.Features.Ui.Where(f => includeNonPublic || f.Public == true);
        }
    }
}
