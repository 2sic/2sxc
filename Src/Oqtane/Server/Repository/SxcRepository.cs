using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using ToSic.Sxc.Models;

namespace ToSic.Sxc.Repository
{
    public class SxcRepository : ISxcRepository, IService
    {
        private readonly SxcContext _db;

        public SxcRepository(SxcContext context)
        {
            _db = context;
        }

        public IEnumerable<Models.Sxc> GetSxcs(int ModuleId)
        {
            return _db.Sxc.Where(item => item.ModuleId == ModuleId);
        }

        public Models.Sxc GetSxc(int SxcId)
        {
            return _db.Sxc.Find(SxcId);
        }

        public Models.Sxc AddSxc(Models.Sxc Sxc)
        {
            _db.Sxc.Add(Sxc);
            _db.SaveChanges();
            return Sxc;
        }

        public Models.Sxc UpdateSxc(Models.Sxc Sxc)
        {
            _db.Entry(Sxc).State = EntityState.Modified;
            _db.SaveChanges();
            return Sxc;
        }

        public void DeleteSxc(int SxcId)
        {
            Models.Sxc Sxc = _db.Sxc.Find(SxcId);
            _db.Sxc.Remove(Sxc);
            _db.SaveChanges();
        }
    }
}
