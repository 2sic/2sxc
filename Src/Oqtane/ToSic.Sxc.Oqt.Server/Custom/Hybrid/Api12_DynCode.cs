using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using static ToSic.Eav.Parameters;
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

        [PrivateApi] public int CompatibilityLevel => Constants.CompatibilityLevel12;

        public new TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        /// <inheritdoc cref="IDynamicCode.App" />
        public IApp App => _DynCodeRoot?.App;

        /// <inheritdoc cref="IDynamicCode.Data" />
        public IContextData Data => _DynCodeRoot?.Data;

        #region AsDynamic implementations
        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        [NonAction]
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot?.AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        [NonAction]
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsC.AsDynamic(entity);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
        [NonAction]
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsC.AsDynamicInternal(dynamicEntity);

        /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        [NonAction]
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsC.MergeDynamic(entities);

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsC.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc cref="IDynamicCode.AsList" />
        [NonAction]
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsC.AsDynamicList(list);

        #endregion

        #region Convert-Service
        [PrivateApi] public IConvertService Convert => _DynCodeRoot.Convert;

        #endregion


        #region CreateSource implementations

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
        [NonAction]
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
        [NonAction]
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public new dynamic Content => _DynCodeRoot?.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot?.Header;


        #endregion

        #region Adam

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        [NonAction]
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot?.AsAdam(item, fieldName);

        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        [NonAction]
        public ToSic.Sxc.Adam.IFile SaveInAdam(string noParamOrder = Protector,
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

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;

        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot?.Edit;

        #endregion

        #region  CreateInstance implementation

        string IGetCodePath.CreateInstancePath { get; set; }

        [NonAction]
        public dynamic CreateInstance(string virtualPath,
            string noParamOrder = Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
            => _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name, (this as IGetCodePath).CreateInstancePath, throwOnError);

        #endregion


        #region CmsContext

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc cref="IDynamicCode12.Settings" />
        public dynamic Settings => _DynCodeRoot.Settings;

        // TODO: MOVE OUT WITH CODE REFACTORING

        /// <inheritdoc />
        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc />
        public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

        /// <inheritdoc />
        public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

        /// <inheritdoc />
        public ICmsView MyView => _DynCodeRoot.CmsContext.View;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion
    }
}
