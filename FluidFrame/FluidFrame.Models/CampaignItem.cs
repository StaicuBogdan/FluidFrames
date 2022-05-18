using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.Models
{
    public class CampaignItem
    {
        public int Id { get; set; }
        [Required]
        public int CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        [ValidateNever]
        public Campaign Campaign { get; set; }

        [Required]
        public int FrameId { get; set; }
        [ForeignKey("FrameId")]
        [ValidateNever]
        public Frame Frame { get; set; }
        public int DaysBooked { get; set; }
        public double Price { get; set; }
    }
}
