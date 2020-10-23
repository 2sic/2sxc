using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Server.Repository
{
    public interface ISxcRepository
    {
        IEnumerable<Shared.Models.Sxc> GetSxcs(int ModuleId);
        Shared.Models.Sxc GetSxc(int SxcId);
        Shared.Models.Sxc AddSxc(Shared.Models.Sxc Sxc);
        Shared.Models.Sxc UpdateSxc(Shared.Models.Sxc Sxc);
        void DeleteSxc(int SxcId);
    }
}
