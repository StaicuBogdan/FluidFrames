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
    public class CampaignItemRepository : Repository<CampaignItem>, ICampaignItemRepository
    {
        private ApplicationDbContext _db;

        public CampaignItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CampaignItem obj)
        {
            _db.CampaignItems.Update(obj);
        }
    }
}
