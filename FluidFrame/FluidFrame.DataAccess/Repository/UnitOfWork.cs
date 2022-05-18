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
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public ICampaignRepository Campaign { get; private set; }
        public ICampaignItemRepository CampaignItem { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            FrameType = new FrameTypeRepository(_db);
            Frame = new FrameRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Campaign = new CampaignRepository(_db);
            CampaignItem = new CampaignItemRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
