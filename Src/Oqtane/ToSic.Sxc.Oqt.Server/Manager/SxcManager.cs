using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using ToSic.Sxc.Oqt.Server.Repository;

namespace ToSic.Sxc.Oqt.Server.Manager
{
    public class SxcManager : IInstallable, IPortable
    {
        private ISxcRepository _SxcRepository;
        private ISqlRepository _sql;

        public SxcManager(ISxcRepository SxcRepository, ISqlRepository sql)
        {
            _SxcRepository = SxcRepository;
            _sql = sql;
        }

        public bool Install(Tenant tenant, string version)
        {
            return _sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc." + version + ".sql");
        }

        public bool Uninstall(Tenant tenant)
        {
            return _sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");
        }

        public string ExportModule(Module module)
        {
            string content = "";
            List<Shared.Models.Sxc> Sxcs = _SxcRepository.GetSxcs(module.ModuleId).ToList();
            if (Sxcs != null)
            {
                content = JsonSerializer.Serialize(Sxcs);
            }
            return content;
        }

        public void ImportModule(Module module, string content, string version)
        {
            List<Shared.Models.Sxc> Sxcs = null;
            if (!string.IsNullOrEmpty(content))
            {
                Sxcs = JsonSerializer.Deserialize<List<Shared.Models.Sxc>>(content);
            }
            if (Sxcs != null)
            {
                foreach(var Sxc in Sxcs)
                {
                    _SxcRepository.AddSxc(new Shared.Models.Sxc { ModuleId = module.ModuleId, Name = Sxc.Name });
                }
            }
        }
    }
}
