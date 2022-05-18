using FluidFrame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.DataAccess.Repository.IRepository
{
    public interface ICampaignItemRepository : IRepository<CampaignItem>
    {
        void Update(CampaignItem obj);
    }
}
