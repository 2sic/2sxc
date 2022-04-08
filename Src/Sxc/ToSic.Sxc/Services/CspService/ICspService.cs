using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Services
{
    public interface ICspService
    {
        bool ReportOnly { get; set; }
        void Add(string name, params string[] values);

        string Name { get; }

        void Activate();
        void AddPolicy(string policy);
        //void AddPolicy(CspParameters policy);
        void AddReport(string policy);
        //void AddReport(CspParameters policy);
    }
}