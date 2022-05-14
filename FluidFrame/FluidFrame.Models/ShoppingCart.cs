using System.ComponentModel.DataAnnotations;

namespace FluidFrame.Models
{
    public class ShoppingCart
    {
        public Frame Frame { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }
    }
}
