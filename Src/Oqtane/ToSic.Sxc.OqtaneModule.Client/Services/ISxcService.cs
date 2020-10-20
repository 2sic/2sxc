using System.Collections.Generic;
using System.Threading.Tasks;
using ToSic.Sxc.OqtaneModule.Models;

namespace ToSic.Sxc.OqtaneModule.Services
{
    public interface ISxcService
    {
        Task<List<Models.Sxc>> GetSxcsAsync(int ModuleId);

        Task<Models.Sxc> GetSxcAsync(int SxcId, int ModuleId);

        Task<Models.Sxc> AddSxcAsync(Models.Sxc Sxc);

        Task<Models.Sxc> UpdateSxcAsync(Models.Sxc Sxc);

        Task DeleteSxcAsync(int SxcId, int ModuleId);
    }
}
