using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Services
{
    public interface ICspService
    {
        void Activate();
        void AddPolicy(string policy);
        void AddPolicy(CspParameters policy);
        void AddReport(string policy);
        void AddReport(CspParameters policy);
    }
}