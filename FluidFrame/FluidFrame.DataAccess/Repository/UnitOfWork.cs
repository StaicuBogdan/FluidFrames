using FluidFrame.DataAccess.Data;
using FluidFrame.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IFrameTypeRepository FrameType { get; private set; }
        public IFrameRepository Frame { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            FrameType = new FrameTypeRepository(_db);
            Frame = new FrameRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
