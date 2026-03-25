using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services.PageShield;

[PrivateApi]
public interface IPageShield
{
    public string? Allow(string urlParameters);
    string? ParametersAllowed { get; }
    string ParametersUnexpected { get; }
    IParameters Parameters { get; }
}
