using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface ISxcService
    {
        Task<List<Shared.Models.Sxc>> GetSxcsAsync(int ModuleId);

        Task<Shared.Models.Sxc> GetSxcAsync(int SxcId, int ModuleId);

        Task<Shared.Models.Sxc> AddSxcAsync(Shared.Models.Sxc Sxc);

        Task<Shared.Models.Sxc> UpdateSxcAsync(Shared.Models.Sxc Sxc);

        Task DeleteSxcAsync(int SxcId, int ModuleId);
    }
}
