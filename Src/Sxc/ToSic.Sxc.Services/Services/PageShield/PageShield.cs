using ToSic.Razor.Blade;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.PageServiceShared;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Services.PageShield;

internal class PageShield(IPageServiceShared pageServiceShared, IUser user)
    : ServiceWithContext("Sxc.OutCac", connect: [pageServiceShared]), IPageShield
{

    private IPageServiceSharedInternal PssInternal => (IPageServiceSharedInternal)pageServiceShared;

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

    // TODO: this can't work yet...
    public IHtmlTag? Enforce(ILinkService link, string? prioritize = null)
    {
        if (ParametersAreValid)
            return null;

        if (user.IsContentEditor)
            return Tag.Div().Class("alert alert-danger").Wrap(
                Tag.H4("Warning: Unexpected URL Parameters Detected"),
                Tag.P(
                    "The URL contains parameters which are not expected on this page, and may cause problems. If you are not logged it, this will trigger a redirect. Please check the URL and remove any parameters which are not expected, or update the configuration to expect these parameters.",
                    Tag.Br(),
                    "Please check the URL and remove any parameters which are not expected, or update the configuration to expect these parameters."
                ),
                Tag.Ol().Wrap(
                    Tag.Li(Tag.Strong("Expected"), ": ", Tag.Code(Parameters + "")),
                    Tag.Li(Tag.Strong("Actual"), ": ", Tag.Code(PageParametersSafe + "")),
                    Tag.Li(Tag.Strong("Unexpected"), ": ", Tag.Code(ParametersUnexpected + ""))
                )
            );

        var parameters = Parameters;
        if (prioritize != null)
            parameters = parameters.Prioritize(prioritize);

#if NETFRAMEWORK
        var response = System.Web.HttpContext.Current.Response;

        // Clear any previous headers/content
        response.Clear();

        // Manually set the 301 status
        response.StatusCode = 301;
        response.StatusDescription = "Moved Permanently";

        // Set the destination
        response.AddHeader("Location", link.To(parameters: parameters));

        // End the response to prevent further processing
        response.End();

#endif
        return null;
    }
}
