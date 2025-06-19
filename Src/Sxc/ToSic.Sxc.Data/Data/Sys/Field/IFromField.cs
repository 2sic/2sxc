namespace ToSic.Sxc.Data.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IFromField
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IField Field { get; set; }
}