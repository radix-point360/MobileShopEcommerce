using System.ComponentModel.DataAnnotations;

namespace MobileShop.ViewModel
{
    public class ReviewViewModel
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }
}
