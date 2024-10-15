using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

// TODO: @STV - PLS EXPLAIN what this does / what it's for
/**
     * This is adjusted version of Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.
     * For reference, original AuthorizationMiddleware class is commented on the bottom of this file.
     * Our version is made because endpoint metadata is provided from our custom ActionContext.
     */
internal class AppApiAuthorization: IHasLog
{
    public AppApiAuthorization(ILogStore logStore)
    {
        Log = new Log(HistoryLogName, null, "AppApiAuthorization");
        logStore.Add(HistoryLogGroup, Log);
    }

    public AppApiAuthorization Init(RequestDelegate next)
    {
        _next = next;
        return this;
    }

    public ILog Log { get; }
    protected string HistoryLogGroup { get; } = "app-api";
    protected static string HistoryLogName => "Authorization";

    private const string SuppressUseHttpContextAsAuthorizationResource = "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

    // Property key is used by Endpoint routing to determine if Authorization has run
    private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";

    private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new();

    private RequestDelegate _next = (HttpContext _) => Task.CompletedTask;

    /**
         * Invoke is adjusted to work with ActionContext instead of HttpContext in AuthorizationMiddleware.
         */
    public async Task Invoke(ActionContext actionContext)
    {
        if (actionContext == null)
        {
            throw new ArgumentNullException(nameof(actionContext));
        }

        var context = actionContext.HttpContext;

        var policyProvider = context.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();

        // Code bellow is copy from Invoke in AuthorizationMiddleware.

        var endpoint = context.GetEndpoint();

        if (endpoint != null)
        {
            // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
            // The Authorization middleware can only make this claim if it observes an actual endpoint.
            context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
        }

        // IMPORTANT: Changes to authorization logic should be mirrored in MVC's AuthorizeFilter
        //var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
        // Original code is commented because endpoint.Metadata is from middleware.
        // In our version endpoint metadata should come from custom web api,
        // so it is provided from ActionContext.ActionDescriptor.EndpointMetadata.
        var authorizeData = actionContext.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
        var policy = await AuthorizationPolicy.CombineAsync(policyProvider, authorizeData);
        if (policy == null)
        {
            await _next(context);
            return;
        }

        // Policy evaluator has transient lifetime so it fetched from request services instead of injecting in constructor
        var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

        var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

        // Allow Anonymous skips all authorization
        //if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        // Original code is commented because endpoint.Metadata is from middleware.
        // In our version endpoint metadata should come from custom web api,
        // so it is provided from ActionContext.ActionDescriptor.EndpointMetadata.
        if (actionContext.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
        {
            await _next(context);
            return;
        }

        object resource;
        if (AppContext.TryGetSwitch(SuppressUseHttpContextAsAuthorizationResource, out var useEndpointAsResource) &&
            useEndpointAsResource)
        {
            resource = endpoint;
        }
        else
        {
            resource = context;
        }

        var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource);
        var authorizationMiddlewareResultHandler =
            context.RequestServices.GetRequiredService<IAuthorizationMiddlewareResultHandler>();

        await authorizationMiddlewareResultHandler.HandleAsync(_next, context, policy, authorizeResult);
    }
}


//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization.Policy;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;

//namespace Microsoft.AspNetCore.Authorization
//{
//    /// <summary>
//    /// A middleware that enables authorization capabilities.
//    /// </summary>
//    public class AuthorizationMiddleware
//    {
//        // AppContext switch used to control whether HttpContext or endpoint is passed as a resource to AuthZ
//        private const string SuppressUseHttpContextAsAuthorizationResource = "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

//        // Property key is used by Endpoint routing to determine if Authorization has run
//        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
//        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

//        private readonly RequestDelegate _next;
//        private readonly IAuthorizationPolicyProvider _policyProvider;

//        /// <summary>
//        /// Initializes a new instance of <see cref="AuthorizationMiddleware"/>.
//        /// </summary>
//        /// <param name="next">The next middleware in the application middleware pipeline.</param>
//        /// <param name="policyProvider">The <see cref="IAuthorizationPolicyProvider"/>.</param>
//        public AuthorizationMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider) 
//        {
//            _next = next ?? throw new ArgumentNullException(nameof(next));
//            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
//        }

//        /// <summary>
//        /// Invokes the middleware performing authorization.
//        /// </summary>
//        /// <param name="context">The <see cref="HttpContext"/>.</param>
//        public async Task Invoke(HttpContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            var endpoint = context.GetEndpoint();

//            if (endpoint != null)
//            {
//                // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
//                // The Authorization middleware can only make this claim if it observes an actual endpoint.
//                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
//            }

//            // IMPORTANT: Changes to authorization logic should be mirrored in MVC's AuthorizeFilter
//            var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
//            var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
//            if (policy == null)
//            {
//                await _next(context);
//                return;
//            }

//            // Policy evaluator has transient lifetime so it fetched from request services instead of injecting in constructor
//            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

//            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

//            // Allow Anonymous skips all authorization
//            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
//            {
//                await _next(context);
//                return;
//            }

//            object? resource;
//            if (AppContext.TryGetSwitch(SuppressUseHttpContextAsAuthorizationResource, out var useEndpointAsResource) && useEndpointAsResource)
//            {
//                resource = endpoint;
//            }
//            else
//            {
//                resource = context;
//            }
            
//            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource);
//            var authorizationMiddlewareResultHandler = context.RequestServices.GetRequiredService<IAuthorizationMiddlewareResultHandler>();
//            await authorizationMiddlewareResultHandler.HandleAsync(_next, context, policy, authorizeResult);
//        }
//    }
//}

