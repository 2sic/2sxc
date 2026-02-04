using ToSic.Sxc.Web.Sys.ClientAssets;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record RenderEngineResult: RenderEngineResultRaw
{
    //public required string Html { get; init; }

    public required bool ActivateJsApi { get; init; }

    [field: AllowNull, MaybeNull]
    public List<ClientAsset> Assets
    {
        get => field ??= [];
        init;
    }

    public string? ErrorCode { get; init; }

    //public List<Exception>? ExceptionsOrNull { get; init; }
}

public record RenderEngineResultRaw
{
    public required string Html { get; init; }
    public List<Exception>? ExceptionsOrNull { get; init; }

}