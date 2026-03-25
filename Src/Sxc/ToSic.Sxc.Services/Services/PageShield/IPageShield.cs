using ToSic.Razor.Blade;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services.PageShield;

[PrivateApi]
public interface IPageShield
{
    public string? Allow(string keys, string? values = null);

    string? ParametersAllowed { get; }
    IParameters ParametersUnexpected { get; }
    IParameters Parameters { get; }
    bool ParametersAreValid { get; }
    IHtmlTag? Enforce(ILinkService link);
}
