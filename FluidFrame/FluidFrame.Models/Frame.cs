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
        public string ImageUrl { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }


        [Required]
        [Range(1, 99999)]
        public double Price { get; set; }
        [Required]
        [Range(1, 99999)]
        public double Price7Days { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        public int FrameTypeId { get; set; }
        public FrameType FrameType { get; set; }
    }
}
