using System;
using System.Collections.Generic;
using ToSic.Eav.DataSources;
using System.IO;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi
{
    /// <inheritdoc cref="SxcApiControllerBase" />
    /// <summary>
    /// This is the base class for API Controllers which need the full context
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [SxcWebApiExceptionHandling]
    public abstract class SxcApiController : DynamicApiController, IDynamicWebApi, IDynamicCodeBeforeV10
    {
        public new IDnnContext Dnn => base.Dnn;//  DynCodeHelpers.Dnn;

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
        [PrivateApi("old api, only available in old API controller")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  DynCodeHelpers.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  DynCodeHelpers.AsDynamic(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) =>  DynCodeHelpers.AsEntity(dynamicEntity);

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  DynCodeHelpers.AsDynamic(entities);
        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => DynCodeHelpers.AsDynamic(entity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => DynCodeHelpers.AsDynamic(entityKeyValuePair.Value);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => DynCodeHelpers.AsDynamic(entities);
        #endregion


        #region CreateSource implementations
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        ITokenListFiller configurationProvider = null)
	        => DynCodeHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, ITokenListFiller configurationProvider = null)
            where T : IDataSource
            =>  DynCodeHelpers.CreateSource<T>(inSource, configurationProvider);

	    public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
            => DynCodeHelpers.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List
        /// <summary>
        /// content item of the current view
        /// </summary>
        public dynamic Content => DynCodeHelpers.Content;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Content.Presentation instead")]
        public dynamic Presentation => DynCodeHelpers.Content?.Presentation;

        public dynamic Header => DynCodeHelpers.Header;

        [Obsolete("use Header instead")]
	    public dynamic ListContent => DynCodeHelpers.Header;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Header.Presentation instead")]
	    public dynamic ListPresentation => DynCodeHelpers.Header?.Presentation;

        [Obsolete("This is an old way used to loop things. Use Data[\"Default\"] instead. Will be removed in 2sxc v10")]
        public List<Element> List => DynCodeHelpers.List;

        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
	        => DynCodeHelpers.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCodeHelpers.AsAdam(entity, fieldName);


        /// <summary>
        /// Save a file from a stream (usually an upload from the browser) into an adam-field
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">ensure that all parameters use names, so the api can change in future</param>
        /// <param name="stream">the stream</param>
        /// <param name="fileName">file name to save to</param>
        /// <param name="contentType">content-type of the target item (important for security checks)</param>
        /// <param name="guid"></param>
        /// <param name="field"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public new Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => base.SaveInAdam(dontRelyOnParameterOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion

        #region Link & Edit - added in 2sxc 10.01
        public ILinkHelper Link => DynCodeHelpers?.Link;
        public IInPageEditingSystem Edit => DynCodeHelpers?.Edit;

        #endregion

    }
}
