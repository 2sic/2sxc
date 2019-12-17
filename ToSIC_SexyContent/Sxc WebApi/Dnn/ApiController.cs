using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the base class for all custom API Controllers. <br/>
    /// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
    /// </summary>
    [PublicApi]
    [SxcWebApiExceptionHandling]
    public abstract partial class ApiController : DynamicApiController, IDynamicWebApi, IDynamicCodeBeforeV10
    {
        /// <inheritdoc />
        public new IDnnContext Dnn => base.Dnn;// DynCodeHelpers.Dnn;

        [PrivateApi("try to remove")]
        public SxcHelper Sxc => DynCode.Sxc;

        /// <inheritdoc />
        public IApp App => DynCode.App;

        /// <inheritdoc />
        public IBlockDataSource Data => DynCode.Data;


        #region AsDynamic implementations
        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynCode.AsDynamic(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DynCode.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  DynCode.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) =>  DynCode.AsEntity(dynamicEntity);

        #endregion

        #region AsList (experimental)

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list) 
            => DynCode?.AsList(list);

        #endregion


        #region CreateSource implementations

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            =>  DynCode.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
	    public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
            => DynCode.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public dynamic Content => DynCode.Content;

        /// <inheritdoc />
        public dynamic Header => DynCode.Header;


        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCode.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public new Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => base.SaveInAdam(dontRelyOnParameterOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc />
        public ILinkHelper Link => DynCode?.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCode?.Edit;

        #endregion

    }
}
