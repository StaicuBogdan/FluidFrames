using FluidFrame.DataAccess.Data;
using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.DataAccess.Repository
{
    public class FrameTypeRepository : Repository<FrameType>, IFrameTypeRepository
    {
        private ApplicationDbContext _db;

        public FrameTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(FrameType obj)
        {
            _db.FrameTypes.Update(obj);
        }
    }
}
