using System.Collections.Generic;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadPrefetchHelper
    {
        private List<JsonEntity> PrefetchSettingsEntities() => Log.Func(l =>
        {
            return new List<JsonEntity> { };

        });

    }
}
