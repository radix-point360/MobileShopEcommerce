using MobileShop.Models;

namespace MobileShop.ViewModel
{
    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
        public List<Product> NewArrivals { get; set; } = new List<Product>();
        public List<Product> Bestsellers { get; set; } = new List<Product>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
    }
}
