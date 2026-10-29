using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record AdamItemDtoMakerOptions
{
    public AdamContext? AdamContext { get; init; }
}