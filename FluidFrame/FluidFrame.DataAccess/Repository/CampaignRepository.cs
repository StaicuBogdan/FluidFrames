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
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        private ApplicationDbContext _db;

        public CampaignRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Campaign obj)
        {
            _db.Campaigns.Update(obj);
        }
    }
}
