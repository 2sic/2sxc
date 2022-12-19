using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Configuration
{
    /// <summary>
    /// WIP
    /// </summary>
    [PrivateApi]
    public interface ISettingsForEditUi
    {
        IList<KeyValuePair<string, object>> GetUiSettings();
    }
}
