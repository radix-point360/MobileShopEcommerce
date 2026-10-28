using MobileShop.Models;

namespace MobileShop.ViewModel
{
    public class CompareProductsViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<string> SpecificationNames { get; set; } = new();
    }
}
