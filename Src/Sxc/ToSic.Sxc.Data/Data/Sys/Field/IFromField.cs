namespace ToSic.Sxc.Data.Sys.Field;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IFromField
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IField Field { get; set; }
}