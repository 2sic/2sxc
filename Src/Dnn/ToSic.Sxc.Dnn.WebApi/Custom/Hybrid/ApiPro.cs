﻿using System.Collections.Generic;
using System;
using ToSic.Eav;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;
using System.Web.Http;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic WebAPI files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [PublicApi]
    [DnnLogExceptions]
    // v16 should now default to normal
    //[DefaultToNewtonsoftForHttpJson]
    public abstract class ApiPro: DynamicApiController, /*IDynamicCode14<object, ServiceKit14>,*/ IHasCodeLog,
        IDynamicWebApi,

        //Api14<dynamic, ServiceKit14>,
        //IDynamicCode12, 
        //IDynamicWebApi, 
        IHasDynamicCodeRoot,
        IDynamicCode16
    {
        #region Setup

        protected ApiPro() : base("Hyb14") { }

        protected ApiPro(string logSuffix) : base(logSuffix) { }

        /// <inheritdoc cref="IDynamicCodeKit{ServiceKit14}.Kit" />
        public ServiceKit14 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();

        /// <inheritdoc cref="IHasCodeLog.Log" />
        public new ICodeLog Log => SysHlp.CodeLog;

        [PrivateApi] public int CompatibilityLevel => ToSic.Sxc.Constants.CompatibilityLevel16;

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01; CmsContext, Resources, Settings (v12)

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;

        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        ///// <inheritdoc cref="IDynamicCode.Edit" />
        //public IEditService Edit => _DynCodeRoot?.Edit;

        ///// <inheritdoc cref="IDynamicCode.CmsContext" />
        //public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;
        ////public ICmsContext MyContext => _DynCodeRoot?.CmsContext;

        ///// <inheritdoc cref="IDynamicCode12.Resources" />
        //public dynamic Resources => _DynCodeRoot.Resources;

        ///// <inheritdoc cref="IDynamicCode12.Settings" />
        //public dynamic Settings => _DynCodeRoot.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion


        #region AsDynamic implementations + AsList - all killed in v16

        ///// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        //public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

        ///// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        //public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsC.AsDynamic(entity);

        ///// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
        //public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsC.AsDynamicInternal(dynamicEntity);

        ///// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        //public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsC.MergeDynamic(entities);

        ///// <inheritdoc cref="IDynamicCode.AsEntity" />
        //public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsC.AsEntity(dynamicEntity);

        ///// <inheritdoc cref="IDynamicCode.AsList" />
        //public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsC.AsDynamicList(list);

        #endregion

        #region Adam

        ///// <inheritdoc cref="IDynamicCode.AsAdam" />
        //public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

        /// <inheritdoc cref="IDynamicWebApi.SaveInAdam" />
        public new ToSic.Sxc.Adam.IFile SaveInAdam(string noParamOrder = ToSic.Eav.Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => base.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion


        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        #endregion

        private CodeInfoService CcS => _ccs.Get(SysHlp.GetService<CodeInfoService>);
        private readonly GetOnce<CodeInfoService> _ccs = new GetOnce<CodeInfoService>();

        ///// <inheritdoc />
        //public new ITypedStack Settings => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Settings);

        ///// <inheritdoc />
        //public new ITypedStack Resources => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Resources);

        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)_DynCodeRoot.App;

        /// <inheritdoc />
        public ITypedStack SettingsStack => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack ResourcesStack => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public ITypedStack SysSettings => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack SysResources => _DynCodeRoot.Resources;

        #endregion

        #region My... Stuff

        private TypedCode16Helper CodeHelper => _codeHelper ?? (_codeHelper = CreateCodeHelper());
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper()
        {
            return new TypedCode16Helper(_DynCodeRoot, MyData, null, false, "c# WebApiController");
        }

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        public IMyData MyData => _DynCodeRoot.Data as IMyData;

        #endregion


        #region AsItem(s) / Merge

        /// <inheritdoc />
        public ITypedStack Merge(params object[] items) => _DynCodeRoot.AsC.AsStack(items);
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.AsStack(items);


        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        #endregion

        public ITypedModel MyModel => CodeHelper.MyModel;

        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.AsC.AsEntity(thing);

        /// <inheritdoc />
        public ITypedRead Read(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);


        #region Net Core Compatibility Shims - Copy this entire section to WebApi Files

        /// <inheritdoc cref="IDynamicWebApi.File"/>
        public dynamic File(string noParamOrder = ToSic.Eav.Parameters.Protector,
            bool? download = null,
            string virtualPath = null,
            string contentType = null,
            string fileDownloadName = null,
            object contents = null)
            => Shim.File(noParamOrder, download, virtualPath, contentType, fileDownloadName, contents);

        private WebApiCoreShim Shim => new WebApiCoreShim(Request);

        /// <inheritdoc cref="WebApiCoreShim.Ok()"/>
        [NonAction] public new dynamic Ok() => Shim.Ok();

        /// <inheritdoc cref="WebApiCoreShim.Ok(object)"/>
        [NonAction] public dynamic Ok(object value) => Shim.Ok(value);

        /// <inheritdoc cref="WebApiCoreShim.NoContent()"/>
        [NonAction]
        public dynamic NoContent() => Shim.NoContent();

        // TODO: this Shim could now be implemented after 16.02 - since we don't have the Content property any more
        #region Content (ca. 5 overloads) can't be implemented, because it conflicts with our property "Content"

        #endregion

        /// <inheritdoc cref="WebApiCoreShim.Redirect"/>
        [NonAction] public new dynamic Redirect(string url) => Shim.Redirect(url);

        /// <inheritdoc cref="WebApiCoreShim.RedirectPermanent"/>
        [NonAction] public dynamic RedirectPermanent(string url) => Shim.RedirectPermanent(url);


        /// <inheritdoc cref="WebApiCoreShim.StatusCode(int)"/>
        [NonAction] public dynamic StatusCode(int statusCode) => Shim.StatusCode(statusCode);

        /// <inheritdoc cref="WebApiCoreShim.StatusCode(int, object)"/>
        [NonAction] public dynamic StatusCode(int statusCode, object value) => Shim.StatusCode(statusCode, value);


        /// <inheritdoc cref="WebApiCoreShim.Unauthorized()"/>
        [NonAction] public dynamic Unauthorized() => Shim.Unauthorized();

        /// <inheritdoc cref="WebApiCoreShim.Unauthorized(object)"/>
        [NonAction] public dynamic Unauthorized(object value) => Shim.Unauthorized(value);

        /// <inheritdoc cref="WebApiCoreShim.NotFound()"/>
        [NonAction] public new dynamic NotFound() => Shim.NotFound();

        /// <inheritdoc cref="WebApiCoreShim.NotFound(object)"/>
        [NonAction] public dynamic NotFound(object value) => Shim.NotFound(value);

        /// <inheritdoc cref="WebApiCoreShim.BadRequest()"/>
        [NonAction] public new dynamic BadRequest() => Shim.BadRequest();

        /// <inheritdoc cref="WebApiCoreShim.Conflict()"/>
        [NonAction] public new dynamic Conflict() => Shim.Conflict();

        /// <inheritdoc cref="WebApiCoreShim.Conflict(object)"/>
        [NonAction] public dynamic Conflict(object error) => Shim.Conflict(error);

        /// <inheritdoc cref="WebApiCoreShim.Accepted()"/>
        [NonAction] public dynamic Accepted() => Shim.Accepted();

        /// <inheritdoc cref="WebApiCoreShim.Forbid()"/>
        [NonAction] public dynamic Forbid() => Shim.Forbid();

        #endregion

    }
}