using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Web.Sys.EditUi;

namespace ToSic.Sxc.Oqt.Server.StartUp;

/// <summary>
/// Initializes the intended site for edit UI documents before Oqtane resolves the tenant and validates the login cookie.
/// </summary>
/// <remarks>
/// The editor uses root-relative /Modules/... URLs, so Oqtane's URL-based resolution would select the root site.
/// Its cookie validator (PrincipalValidator.ValidateAsync) then checks the user's membership in that site
/// and can sign out a user who only belongs to the requesting subsite.
/// Resolving the alias inside EditUiMiddleware is too late, because that endpoint runs after UseAuthentication.
/// This filter prepares the alias first; Oqtane still performs its normal authentication and authorization checks.
/// </remarks>
internal class OqtEditUiStartupFilter : IStartupFilter
{
    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        => app =>
        {
            // Register before next(app), which adds the downstream Oqtane middleware.
            app.Use(async (context, nextMiddleware) =>
            {
                InitializeAlias(context);
                await nextMiddleware(context);
            });
            next(app);
        };

    private static void InitializeAlias(HttpContext context)
    {
        var request = context.Request;
        // Only editor documents need this override; assets and other routes keep Oqtane's normal site resolution.
        if (!OqtWebApiConstants.SxcDialogs.Any(dialog =>
                string.Equals(request.Path.Value?.TrimEnd('/'), dialog.url.TrimEnd('/'), StringComparison.OrdinalIgnoreCase)))
            return;

        // Both IDs come from the request. Only accept an existing alias belonging to the specified tenant.
        if (!int.TryParse(request.Query[HtmlDialog.AliasIdInUrl], out var aliasId)
            || !int.TryParse(request.Query[HtmlDialog.TenantIdInUrl], out var tenantId))
            return;

        var alias = context.RequestServices.GetRequiredService<IAliasRepository>().GetAlias(aliasId, tracking: false);
        if (alias == null || alias.TenantId != tenantId)
            return;

        // Seed the request-scoped SiteState so TenantMiddleware can load the correct site's settings.
        // Leave HttpContext.Items to TenantMiddleware: it adds the alias key itself, before authentication.
        context.RequestServices.GetRequiredService<ITenantManager>().SetAlias(alias);
    }
}
