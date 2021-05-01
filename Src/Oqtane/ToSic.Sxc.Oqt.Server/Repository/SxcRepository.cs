using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.Server.Repository
{
    public class SxcRepository : ISxcRepository, IService
    {
        private readonly SxcContext _db;

        public SxcRepository(SxcContext context)
        {
            _db = context;
        }

        public IEnumerable<Shared.Models.SxcRepositoryObjectUnclearIfUsed> GetSxcs(int ModuleId)
        {
            return _db.Sxc.Where(item => item.ModuleId == ModuleId);
        }

        public Shared.Models.SxcRepositoryObjectUnclearIfUsed GetSxc(int SxcId)
        {
            return _db.Sxc.Find(SxcId);
        }

        public Shared.Models.SxcRepositoryObjectUnclearIfUsed AddSxc(Shared.Models.SxcRepositoryObjectUnclearIfUsed sxcRepositoryObjectUnclearIfUsed)
        {
            _db.Sxc.Add(sxcRepositoryObjectUnclearIfUsed);
            _db.SaveChanges();
            return sxcRepositoryObjectUnclearIfUsed;
        }

        public Shared.Models.SxcRepositoryObjectUnclearIfUsed UpdateSxc(Shared.Models.SxcRepositoryObjectUnclearIfUsed sxcRepositoryObjectUnclearIfUsed)
        {
            _db.Entry(sxcRepositoryObjectUnclearIfUsed).State = EntityState.Modified;
            _db.SaveChanges();
            return sxcRepositoryObjectUnclearIfUsed;
        }

        public void DeleteSxc(int SxcId)
        {
            Shared.Models.SxcRepositoryObjectUnclearIfUsed sxcRepositoryObjectUnclearIfUsed = _db.Sxc.Find(SxcId);
            _db.Sxc.Remove(sxcRepositoryObjectUnclearIfUsed);
            _db.SaveChanges();
        }
    }
}
