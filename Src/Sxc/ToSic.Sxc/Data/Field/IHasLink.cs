using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Helper to handle generic cases where something should have a url, but it could be a string or a smarter object")]
    public interface IHasLink
    {
        string Url { get; }
    }
}
