using System;
using System.IO;
using System.Web.Http.Controllers;
using ToSic.Lib.Documentation;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This is the foundation for both the old SxcApiController and the new Dnn.ApiController.
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [PrivateApi("This is an internal base class used for the App ApiControllers. Make sure the implementations don't break")]
    // Note: 2022-02 2dm I'm not sure if this was ever published as the official api controller, but it may have been?
    [DnnLogExceptions]
    public abstract class DynamicApiController : SxcApiControllerBase<DummyControllerReal>, ICreateInstance, IHasDynamicCodeRoot
    {
        #region Constructor & DI / Setup

        /// <summary>
        /// Empty constructor is important for inheriting classes
        /// </summary>
        [PrivateApi]
        protected DynamicApiController() : this("DynApi") { }

        [PrivateApi]
        protected DynamicApiController(string logSuffix) : base(logSuffix) { }

        [PrivateApi]
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var init = DynHlp.Initialize(controllerContext);
            ((ICreateInstance)this).CreateInstancePath = init.Path;
            _DynCodeRoot = init.Root;
        }

        #endregion

        #region Internal / Plumbing / Obsolete

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        string ICreateInstance.CreateInstancePath { get; set; }

        /// <summary>
        /// The name of the logger in insights. The inheriting class should provide the real name to be used.
        /// Note: Probably almost never used, except by 2sic. Must determine if we just remove it
        /// </summary>
        [Obsolete("Deprecated in v13.03 - doesn't serve a purpose any more. Will just remain to avoid breaking public uses of this property.")]
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        [PrivateApi] protected virtual string HistoryLogName { get; }

        #endregion

        #region Services / Properties to share

        /// <inheritdoc cref="IDynamicCode.GetService{TService}"/>
        public virtual TService GetService<TService>() => SysHlp.GetService<TService>();

        /// <inheritdoc cref="IHasDnn.Dnn"/>
        public IDnnContext Dnn => (_DynCodeRoot as IHasDnn)?.Dnn;

        /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
        public Sxc.Adam.IFile SaveInAdam(string noParamOrder = Protector, Stream stream = null, string fileName = null, string contentType = null,
            Guid? guid = null, string field = null, string subFolder = "")
            => DynHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

        /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
        public dynamic CreateInstance(string virtualPath, string noParamOrder = Protector, string name = null, string relativePath = null, bool throwOnError = true)
            => _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name, ((ICreateInstance)this).CreateInstancePath, throwOnError);

        #endregion
    }
}
