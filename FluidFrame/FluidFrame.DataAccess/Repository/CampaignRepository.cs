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

        public void UpdateStatus(int id, string campaignStatus, string? paymentStatus = null)
        {
            var campaignFromDb = _db.Campaigns.FirstOrDefault(u => u.Id == id);
            if (campaignFromDb != null)
            {
                campaignFromDb.CampaignStatus = campaignStatus;
                if (paymentStatus != null)
                {
                    campaignFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var campaignFromDb = _db.Campaigns.FirstOrDefault(u => u.Id == id);
            campaignFromDb.SessionId = sessionId;
            campaignFromDb.PaymentIntentId = paymentIntentId;
            campaignFromDb.PaymentDate = DateTime.Now;
        }
    }
}
