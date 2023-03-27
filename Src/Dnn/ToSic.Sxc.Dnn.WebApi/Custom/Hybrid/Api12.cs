using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;
using static ToSic.Eav.Parameters;
using System;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi("This is the official base class for v12+")]
    [DnnLogExceptions]
    [UseOldNewtonsoftForHttpJson]
    public abstract partial class Api12: ApiCoreShim, 
        IDynamicCode12, 
        IDynamicWebApi, 
        IHasDynamicCodeRoot,
        IHasCodeLog
    {
        protected Api12() : base("Hyb12") { }

        protected Api12(string logSuffix) : base(logSuffix) { }

        #region IHasLog

        /// <inheritdoc />
        public new ICodeLog Log => _codeLog.Get(() => new CodeLog(base.Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        [PrivateApi] ILog IHasLog.Log => base.Log;

        #endregion

        #region CreateDataSource - new in v15, don't use in this old deprecated base class

        [PrivateApi]
        public T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
            => throw new Exception(DynamicCodeConstants.ErrorCreateDataSourceRequiresV14);

        [PrivateApi]
        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = null, object options = default)
            => throw new Exception(DynamicCodeConstants.ErrorCreateDataSourceRequiresV14);

        #endregion

    }
}
