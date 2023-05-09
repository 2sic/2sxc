namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface IOqtPrerenderSupportService
    {
        bool Executed { get; set; }
        bool HasUserAgentSignature();
    }
}