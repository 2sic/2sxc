using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AdamItemDtoMakerOptions
{
    public required AdamContext AdamContext { get; init; }
}