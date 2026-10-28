using MobileShop.Models;
using Stripe.Issuing;

namespace MobileShop.ViewModel
{
    public class ProductListViewModel
    {
        // Data Links
        public List<Product> products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Brand> Brands { get; set; } = new List<Brand>();

        // Filter Parameters
        public int? SelectedCategoryId { get; set; }
        public int? SelectedBrandId { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }

        // Price Range

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Pagination Properties

        public int PageNumber = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

    }
}
