using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /**
     * This is adjusted version of Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.
     * Our version is made because endpoint metadata is provided from our custom ActionContext.
     */
    internal class AppApiSecurity
    {
        private const string SuppressUseHttpContextAsAuthorizationResource = "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

        // Property key is used by Endpoint routing to determine if Authorization has run
        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

        internal static async Task AuthorizeAsync(ActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var context = actionContext.HttpContext;

            var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
                // The Authorization middleware can only make this claim if it observes an actual endpoint.
                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
            }

            // Allow Anonymous skips all authorization
            if (actionContext.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
                return;

            var policyProvider = context.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();
            var authorizeData = actionContext.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
            var policy = await AuthorizationPolicy.CombineAsync(policyProvider, authorizeData);
            if (policy == null)
                return;

            // Policy evaluator has transient lifetime so it fetched from request services instead of injecting in constructor
            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

            object? resource;
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

            static Task Next(HttpContext _) => Task.CompletedTask;

            await authorizationMiddlewareResultHandler.HandleAsync(Next, context, policy, authorizeResult);
        }
        
        
        //internal static async void AuthorizeAsync2(ActionContext actionContext)
        //{
        //    var policyProvider = actionContext.HttpContext.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();

        //    // AllowAnonymousAttribute
        //    if (actionContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
        //        return;

        //    // AuthorizeAttribute
        //    foreach (var authorizeAttribute in actionContext.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>())
        //    {
        //        var policy = await AuthorizationPolicy.CombineAsync(policyProvider, new[] { authorizeAttribute });
        //        if (policy is null) return;
        //        await AuthorizeAsync(actionContext, policy);
        //    }
        //}

        //private static async Task AuthorizeAsync(ActionContext actionContext, AuthorizationPolicy policy)
        //{
        //    var httpContext = actionContext.HttpContext;
        //    var policyEvaluator = httpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
        //    var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, httpContext);
        //    var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, httpContext, actionContext.ActionDescriptor);
        //    if (authorizeResult.Challenged)
        //    {
        //        if (policy.AuthenticationSchemes.Count > 0)
        //        {
        //            foreach (var scheme in policy.AuthenticationSchemes)
        //            {
        //                await httpContext.ChallengeAsync(scheme);
        //            }
        //        }
        //        else
        //        {
        //            await httpContext.ChallengeAsync();
        //        }

        //        return;
        //    }
        //    else if (authorizeResult.Forbidden)
        //    {
        //        if (policy.AuthenticationSchemes.Count > 0)
        //        {
        //            foreach (var scheme in policy.AuthenticationSchemes)
        //            {
        //                await httpContext.ForbidAsync(scheme);
        //            }
        //        }
        //        else
        //        {
        //            await httpContext.ForbidAsync();
        //        }

        //        return;
        //    }
        //}
    }
}
