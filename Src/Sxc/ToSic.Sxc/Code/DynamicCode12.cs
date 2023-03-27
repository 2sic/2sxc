using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// New base class for v12 Dynamic Code
    /// Adds new properties and methods, and doesn't keep old / legacy APIs
    /// </summary>
    [PublicApi]
    public class DynamicCode12: DynamicCode, IDynamicCode12
    {

        #region Convert-Service

        /// <inheritdoc />
        public IConvertService Convert => _DynCodeRoot.Convert;

        #endregion

        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot?.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot?.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        [PrivateApi]
        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSource links = null, object options = null)
            => _DynCodeRoot.CreateDataSource(noParamOrder: name, attach: links, options: options);

    }
}
