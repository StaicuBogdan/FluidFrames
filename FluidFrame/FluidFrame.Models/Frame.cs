using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FluidFrame.Models
{
    public class Frame
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        [Required]
        public string ModelName { get; set; }
        [Required]
        public string OwnerPhoneNumber { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }


        [Required]
        [Range(1, 99999)]
        [Display(Name = "Price for one day")]
        public double Price { get; set; }
        [Required]
        [Range(1, 99999)]
        [Display(Name = "Price for 1 week")]
        public double Price7Days { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }

        [Required]
        [Display(Name = "Frame Type")]
        public int FrameTypeId { get; set; }
        [ValidateNever]
        public FrameType FrameType { get; set; }
    }
}
