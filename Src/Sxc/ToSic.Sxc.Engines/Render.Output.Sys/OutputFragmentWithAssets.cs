using ToSic.Sxc.Web.Sys.ClientAssets;

namespace ToSic.Sxc.Render.Output.Sys;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public record OutputFragmentWithAssets: OutputFragment
{
    public required bool ActivateJsApi { get; init; }

    [field: AllowNull, MaybeNull]
    public List<ClientAsset> Assets
    {
        get => field ??= [];
        init;
    }

    public string? ErrorCode { get; init; }
}