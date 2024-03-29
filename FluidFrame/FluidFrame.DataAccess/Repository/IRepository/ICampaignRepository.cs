﻿using FluidFrame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.DataAccess.Repository.IRepository
{
    public interface ICampaignRepository : IRepository<Campaign>
    {
        void Update(Campaign obj);
        void UpdateStatus(int id, string campaignStatus, string? paymentStatus = null);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
