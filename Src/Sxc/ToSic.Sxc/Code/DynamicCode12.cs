using ToSic.Eav.Documentation;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// New base class for v12 Dynamic Code
    /// Adds new properties & methods, and doesn't keep old / legacy APIs
    /// </summary>
    [PublicApi("WIP / Experimental")]
    public class DynamicCode12: DynamicCode, IDynamicCode12
    {

        #region Convert-Service

        /// <inheritdoc />
        public IConvertService Convert => _contents.Convert;

        #endregion

        /// <inheritdoc />
        public dynamic Resources => _contents?.Resources;

        /// <inheritdoc />
        public dynamic Settings => _contents?.Settings;

        public IDevTools DevTools => _contents.DevTools;
    }
}
