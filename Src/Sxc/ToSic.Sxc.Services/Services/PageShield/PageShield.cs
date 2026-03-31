using ToSic.Razor.Blade;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.PageServiceShared;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Services.PageShield;

internal class PageShield(IPageServiceShared pageServiceShared, IUser user, IHttpContextService httpContextService, ISysFeaturesService features)
    : ServiceWithContext("Sxc.OutCac", connect: [pageServiceShared, httpContextService, features]), IPageShield
{

    private IPageServiceSharedInternal PssInternal => (IPageServiceSharedInternal)pageServiceShared;

    private ILinkService LinkSvc => field ??= ExCtx.GetService<ILinkService>(reuse: true);

    public string Allow(string keys, string? values = null)
    {
        PssInternal.UrlSpecs.Add(keys, values);
        return "";
    }

    public string LoadConfiguration(string configuration)
    {
        PssInternal.UrlSpecs.LoadConfiguration(configuration);
        return "";
    }

    public string ParametersAllowed =>
        PssInternal.UrlSpecs.Keys() ?? "";

    public bool ParametersAreValid =>
        ParametersUnexpected.Count == 0;

    public IParameters ParametersUnexpected =>
        PssInternal.UrlSpecs.GetInvalid(PageParametersSafe);

    public IParameters Parameters =>
        PssInternal.UrlSpecs.GetValid(PageParametersSafe);

    private IParameters PageParametersSafe =>
        ExCtxOrNull?.GetBlock().Context.Page.Parameters ?? new Context.Sys.Parameters();

    [Obsolete]
    public IHtmlTag? Enforce(ILinkService link, string? prioritize = null) =>
        Enforce(prioritizeParameters: prioritize);

    public IHtmlTag? Enforce(NoParamOrder npo = default, string? prioritizeParameters = null)
    {
        var l = Log.Fn<IHtmlTag?>($"{nameof(prioritizeParameters)}:'{prioritizeParameters}'");

        if (!features.IsEnabled(SxcFeatures.PageShieldFloodGates))
            return l.ReturnNull("feature not enabled");

        if (ParametersAreValid)
            return l.ReturnNull("parameters are valid");

        if (user.IsContentEditor)
        {
            var result = Tag.Div().Class("alert alert-danger").Wrap(
                Tag.H4("Warning: Unexpected URL Parameters Detected"),
                Tag.P(
                    """
                    The URL contains parameters which are not expected on this page, and may cause problems. 
                    If you are not logged it, this will trigger a redirect. 
                    """,
                    Tag.Br(),
                    """
                    Please check the URL and remove any parameters which are not expected, 
                    or update the configuration to expect these parameters.
                    """
                ),
                Tag.Ol().Wrap(
                    Tag.Li(Tag.Strong("Expected"), ": ", Tag.Code(Parameters + "")),
                    Tag.Li(Tag.Strong("Actual"), ": ", Tag.Code(PageParametersSafe + "")),
                    Tag.Li(Tag.Strong("Unexpected"), ": ", Tag.Code(ParametersUnexpected + ""))
                )
            );
            return l.Return(result, "showing warning");
        }


        var parameters = Parameters;
        if (prioritizeParameters != null)
            parameters = parameters.Prioritize(prioritizeParameters);

        l.A($"Not editor, will enforce redirect to: {parameters}");

        httpContextService.Redirect301(LinkSvc.To(parameters: parameters));

        return null;
    }
}
