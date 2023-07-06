using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Custom;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using static ToSic.Eav.Parameters;
using Constants = ToSic.Sxc.Constants;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Custom base controller class for custom dynamic 2sxc app api controllers.
    /// It is without dependencies in class constructor, commonly provided with DI.
    /// </summary>
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Api12 : OqtStatefulControllerBase, IDynamicWebApi, IDynamicCode12, IHasCodeLog, IHasDynamicCodeRoot
    {
        #region Setup

        protected Api12() : this(EavWebApiConstants.HistoryNameWebApi) { }

        protected Api12(string logSuffix) : base(logSuffix) { }

        [PrivateApi] public int CompatibilityLevel => Constants.CompatibilityLevel12;

        /// <summary>
        /// Our custom dynamic 2sxc app api controllers, depends on event OnActionExecuting to provide dependencies (without DI in constructor).
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            SysHlp.OnActionExecutingEnd(context);
        }

        #endregion

        #region Infrastructure

        /// <inheritdoc cref="IHasCodeLog.Log" />
        public new ICodeLog Log => SysHlp.CodeLog;

        // ReSharper disable once InconsistentNaming
        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot => SysHlp._DynCodeRoot;

        /// <inheritdoc cref="IDynamicCode.GetService{TService}" />
        public new TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion

        #region App, Data, Settings, Resources, CmsContext

        /// <inheritdoc cref="IDynamicCode.App" />
        public IApp App => _DynCodeRoot?.App;

        /// <inheritdoc cref="IDynamicCode.Data" />
        public IContextData Data => _DynCodeRoot?.Data;

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc cref="IDynamicCode12.Settings" />
        public dynamic Settings => _DynCodeRoot.Settings;

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;


        #endregion


        #region AsDynamic / AsList implementations

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

        /// <inheritdoc cref="IDynamicCode.AsList" />
        [NonAction]
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsC.AsDynamicList(list);

        #endregion

        #region AsEntity

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsC.AsEntity(dynamicEntity);

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

        /// <inheritdoc cref="IDynamicCode.Content" />
        public new dynamic Content => _DynCodeRoot?.Content;

        /// <inheritdoc cref="IDynamicCode.Header" />
        public dynamic Header => _DynCodeRoot?.Header;


        #endregion

        #region Adam

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        [NonAction]
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot?.AsAdam(item, fieldName);

        /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
        [NonAction]
        public IFile SaveInAdam(string noParamOrder = Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => SysHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;

        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot?.Edit;

        #endregion

        #region  CreateInstance implementation

        string IGetCodePath.CreateInstancePath { get; set; }

        /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
        [NonAction]
        public dynamic CreateInstance(string virtualPath, string noParamOrder = Protector, string name = null, string relativePath = null, bool throwOnError = true)
            => _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name, (this as IGetCodePath).CreateInstancePath, throwOnError);

        #endregion

        #region File Response / Download

        /// <inheritdoc cref="IDynamicWebApi.File"/>
        public dynamic File(string noParamOrder = Protector,
            bool? download = null,
            string virtualPath = null,
            string contentType = null,
            string fileDownloadName = null,
            object contents = null
        ) =>
            new OqtWebApiShim(response: Response, this).File(noParamOrder, download, virtualPath, contentType, fileDownloadName, contents);

        #endregion

    }
}
