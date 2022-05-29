using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.Models.ViewModels
{
    public class CampaignViewModel
    {
        public Campaign Campaign { get; set; }
        public IEnumerable<CampaignItem> CampaignItems { get; set; }
    }
}
