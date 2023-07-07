using System;
using System.Net.Http;
using System.Web;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi
{
    internal class DnnWebApiHelper : CodeHelperBase
    {

        #region Constructor / Init

        public DnnWebApiHelper(IHasLog apiController, string historyLogGroup) : base("Dnn.ApiHlp")
        {
            this.LinkLog(apiController.Log);
            WebApiLogging = new DnnWebApiLogging(apiController.Log, GetService<ILogStore>(), historyLogGroup);
        }

        #endregion

        #region Destruct

        public void OnDispose() => WebApiLogging.OnDispose();

        #endregion

        #region Logging

        public DnnWebApiLogging WebApiLogging { get; }


        #endregion

        #region Basic Service

        /// <summary>
        /// Get a service of a specified type. 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// This will override the base functionality to ensure that any services created will be able to get the CodeContext.
        /// </remarks>
        public TService GetService<TService>() => _DynCodeRoot != null
            ? _DynCodeRoot.GetService<TService>()
            : _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
        // Must cache it, to be really sure we use the same ServiceProvider in the same request
        private readonly GetOnce<IServiceProvider> _serviceProvider = new GetOnce<IServiceProvider>();

        public ResponseMakerNetFramework GetResponseMaker() => (ResponseMakerNetFramework)GetService<ResponseMaker<HttpResponseMessage>>();

        #endregion

        /// <summary>
        ///  Extend Time so Web Server doesn't time out
        /// </summary>
        public void PreventServerTimeout300() => HttpContext.Current.Server.ScriptTimeout = 300;


        ///// <summary>
        ///// The RealController which is the full backend of this controller.
        ///// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        ///// </summary>
        //public TRealController Real
        //    => _real.Get(() => GetService<TRealController>()
        //                       ?? throw new Exception($"Can't use {nameof(Real)} for unknown reasons"));
        //private readonly GetOnce<TRealController> _real = new GetOnce<TRealController>();


        public BlockWithContextProvider GetBlockAndContext(HttpRequestMessage request) 
            => _blcCtx.Get(() => GetService<DnnGetBlock>().GetCmsBlock(request));
        private readonly GetOnce<BlockWithContextProvider> _blcCtx = new GetOnce<BlockWithContextProvider>();


    }
}
