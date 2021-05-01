using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Server.Repository
{
    public interface ISxcRepository
    {
        IEnumerable<Shared.Models.SxcRepositoryObjectUnclearIfUsed> GetSxcs(int ModuleId);
        Shared.Models.SxcRepositoryObjectUnclearIfUsed GetSxc(int SxcId);
        Shared.Models.SxcRepositoryObjectUnclearIfUsed AddSxc(Shared.Models.SxcRepositoryObjectUnclearIfUsed sxcRepositoryObjectUnclearIfUsed);
        Shared.Models.SxcRepositoryObjectUnclearIfUsed UpdateSxc(Shared.Models.SxcRepositoryObjectUnclearIfUsed sxcRepositoryObjectUnclearIfUsed);
        void DeleteSxc(int SxcId);
    }
}
