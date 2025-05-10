using ToSic.Sxc.Web.Internal.ClientAssets;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record RenderEngineResult
{
    public required string Html { get; init; }

    public required bool ActivateJsApi { get; init; }

    public List<ClientAsset> Assets
    {
        get => field ??= [];
        init => field = value;
    }

    public string ErrorCode { get; init; }

    public List<Exception> ExceptionsOrNull { get; init; }
}