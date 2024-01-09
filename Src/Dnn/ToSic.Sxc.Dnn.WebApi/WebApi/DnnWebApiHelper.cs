using System;
using System.Net.Http;
using System.Web;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi;

internal class DnnWebApiHelper : CodeHelperBase
{

    #region Constructor / Init

    public DnnWebApiHelper(IHasLog apiController, string historyLogGroup, string firstMessage = default) : base("Dnn.ApiHlp")
    {
        this.LinkLog(apiController.Log);
        WebApiLogging = new DnnWebApiLogging(apiController.Log, GetService<ILogStore>(), historyLogGroup, firstMessage: firstMessage);
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
    public TService GetService<TService>() where TService : class => _DynCodeRoot != null
        ? _DynCodeRoot.GetService<TService>()
        : _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
    // Must cache it, to be really sure we use the same ServiceProvider in the same request
    private readonly GetOnce<IServiceProvider> _serviceProvider = new();

    public void SetupResponseMaker(System.Web.Http.ApiController apiController) => GetService<IResponseMaker>().Init(apiController);

    #endregion

    /// <summary>
    ///  Extend Time so Web Server doesn't time out
    /// </summary>
    public void PreventServerTimeout300() => HttpContext.Current.Server.ScriptTimeout = 300;


    public BlockWithContextProvider GetBlockAndContext(HttpRequestMessage request) 
        => _blcCtx.Get(() => GetService<DnnGetBlock>().GetCmsBlock(request));
    private readonly GetOnce<BlockWithContextProvider> _blcCtx = new();


}