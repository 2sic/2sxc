using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12
    {

        // ReSharper disable once InconsistentNaming
        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot { get; set; }
        // ReSharper disable once InconsistentNaming
        [PrivateApi] public AdamCode _AdamCode { get; set; }

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        /// <inheritdoc />
        public IApp App => _DynCodeRoot?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot?.Data;

        #region AsDynamic implementations
        /// <inheritdoc/>
        [NonAction]
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot?.AsDynamic(json, fallback);

        /// <inheritdoc />
        [NonAction]
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsDynamic(entity);

        /// <inheritdoc />
        [NonAction]
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        [NonAction]
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsDynamic(entities);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        [NonAction]
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsList(list);

        #endregion

        #region Convert-Service
        [PrivateApi] public IConvertService Convert => _DynCodeRoot.Convert;

        #endregion


        #region CreateSource implementations

        /// <inheritdoc />
        [NonAction]
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
        [NonAction]
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public new dynamic Content => _DynCodeRoot?.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot?.Header;


        #endregion

        #region Adam

        /// <inheritdoc />
        [NonAction]
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot?.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        [NonAction]
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot?.AsAdam(entity, fieldName);

        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        [NonAction]
        public ToSic.Sxc.Adam.IFile SaveInAdam(string noParamOrder = Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
        {
            return _AdamCode.SaveInAdam(
                stream: stream,
                fileName: fileName,
                contentType: contentType,
                guid: guid,
                field: field,
                subFolder: subFolder);
        }

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc />
        public ILinkHelper Link => _DynCodeRoot?.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => _DynCodeRoot?.Edit;

        #endregion

        #region  CreateInstance implementation

        public string CreateInstancePath { get; set; }

        [NonAction]
        public dynamic CreateInstance(string virtualPath,
            string noParamOrder = Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name, CreateInstancePath, throwOnError);

        #endregion


        #region CmsContext

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic Settings => _DynCodeRoot.Settings;

        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion
    }
}
