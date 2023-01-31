using System.Collections.Generic;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.WebApi.Cms
{
    public interface ILoadSettingsProvider: IHasLog
    {
        Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters);
    }
}
