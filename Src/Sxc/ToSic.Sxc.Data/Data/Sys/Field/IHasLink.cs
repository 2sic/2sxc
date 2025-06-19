namespace ToSic.Sxc.Data.Sys.Field;

[PrivateApi("Helper to handle generic cases where something should have a url, but it could be a string or a smarter object")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IHasLink
{
    string? Url { get; }
}