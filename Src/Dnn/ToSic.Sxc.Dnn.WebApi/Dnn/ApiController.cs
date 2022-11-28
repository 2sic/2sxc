using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Lib.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi("This was the official base class before v12. Try to move away from it, go to the latest base class on Custom.Dnn.Api12")]
    [DnnLogExceptions]
    [Obsolete("This will continue to work, but you should use the Custom.Hybrid.Api14 or Custom.Dnn.Api12 instead.")]
    [UseOldNewtonsoftForHttpJson]
    public abstract class ApiController : DynamicApiController, 
        IDnnDynamicWebApi, 
        IDynamicCode, 
        IDynamicWebApi, 
        IHasDynamicCodeRoot,
        Eav.Logging.IHasLog
    {
        protected ApiController()
        {
            var log = base.Log.SubLogOrNull("OldApi.DnnApi"); // real log
            _log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }
        
        [PrivateApi]
        public const string ErrRecommendedNamespaces = "To use it, use the new base class from Custom.Hybrid.Api14 or Custom.Dnn.Api12 instead.";

        /// <remarks>
        /// Probably obsolete, but a bit risky to just remove
        /// We will only add it to ApiController but not to Api12, because no new code should ever use that.
        /// </remarks>
        [PrivateApi] public IBlock Block => GetBlock();

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot.Data;


        #region AsDynamic implementations
        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);

        ///// <inheritdoc />
        //[PublicApi("Careful - still Experimental in 12.02")]
        //public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsDynamic(entities);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsEntity(dynamicEntity);

        #endregion


        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsList(list);

        #endregion

        #region CreateSource implementations

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;


        #endregion

        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);


        ///// <inheritdoc />
        //public new ToSic.Sxc.Adam.IFile SaveInAdam(string noParamOrder = ToSic.Eav.Parameters.Protector,
        //    Stream stream = null,
        //    string fileName = null,
        //    string contentType = null,
        //    Guid? guid = null,
        //    string field = null,
        //    string subFolder = "")
        //    => base.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

        public dynamic File(string noParamOrder = Parameters.Protector, bool? download = null, string virtualPath = null,
            string contentType = null, string fileDownloadName = null, object contents = null) =>
            throw new NotSupportedException("Not implemented. " + ErrRecommendedNamespaces);

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc />
        public ILinkService Link => _DynCodeRoot?.Link;

        /// <inheritdoc />
        public IEditService Edit => _DynCodeRoot?.Edit;

        #endregion
        #region RunContext WiP

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;
        #endregion

        #region IHasLog
        public new Eav.Logging.ILog Log => _log ?? (_log = new LogAdapter(null)/*fallback Log*/);

        private Eav.Logging.ILog _log;

        ILog IHasLog.Log => Log.GetContents(); // explicit Log implementation (to ensure that new IHasLog.Log interface is implemented)

        #endregion
    }
}
