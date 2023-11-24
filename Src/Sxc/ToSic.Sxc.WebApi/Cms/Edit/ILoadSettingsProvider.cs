using System.Collections.Generic;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.WebApi.Cms
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface ILoadSettingsProvider: IHasLog
    {
        Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters);
    }
}
