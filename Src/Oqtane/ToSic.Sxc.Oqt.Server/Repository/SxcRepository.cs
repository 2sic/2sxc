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

        public IEnumerable<Shared.Models.Sxc> GetSxcs(int ModuleId)
        {
            return _db.Sxc.Where(item => item.ModuleId == ModuleId);
        }

        public Shared.Models.Sxc GetSxc(int SxcId)
        {
            return _db.Sxc.Find(SxcId);
        }

        public Shared.Models.Sxc AddSxc(Shared.Models.Sxc Sxc)
        {
            _db.Sxc.Add(Sxc);
            _db.SaveChanges();
            return Sxc;
        }

        public Shared.Models.Sxc UpdateSxc(Shared.Models.Sxc Sxc)
        {
            _db.Entry(Sxc).State = EntityState.Modified;
            _db.SaveChanges();
            return Sxc;
        }

        public void DeleteSxc(int SxcId)
        {
            Shared.Models.Sxc Sxc = _db.Sxc.Find(SxcId);
            _db.Sxc.Remove(Sxc);
            _db.SaveChanges();
        }
    }
}
