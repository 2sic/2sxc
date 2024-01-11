namespace ToSic.Sxc.Data.Internal;

[PrivateApi("Helper to handle generic cases where something should have a url, but it could be a string or a smarter object")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IHasLink
{
    string Url { get; }
}