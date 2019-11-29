using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
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

        [PrivateApi]
        public SxcHelper Sxc => DynCodeHelpers.Sxc;

        /// <inheritdoc />
        public IApp App => DynCodeHelpers.App;

        /// <inheritdoc />
        public IBlockDataSource Data => DynCodeHelpers.Data;


        #region AsDynamic implementations

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DynCodeHelpers.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  DynCodeHelpers.AsDynamic(dynamicEntity);



        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  DynCodeHelpers.AsDynamic(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) =>  DynCodeHelpers.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  DynCodeHelpers.AsDynamic(entities);
        #endregion



        #region CreateSource implementations

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ITokenListFiller configurationProvider = null)
            where T : IDataSource
            =>  DynCodeHelpers.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
	    public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
            => DynCodeHelpers.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public dynamic Content => DynCodeHelpers.Content;

        /// <inheritdoc />
        public dynamic Header => DynCodeHelpers.Header;


        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCodeHelpers.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCodeHelpers.AsAdam(entity, fieldName);


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
        public ILinkHelper Link => DynCodeHelpers?.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCodeHelpers?.Edit;

        #endregion

    }
}
