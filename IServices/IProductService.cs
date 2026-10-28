using MobileShop.Models;
using MobileShop.ViewModel;

namespace MobileShop.IServices
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int productId);
        Task<List<Product>> GetRelatedProductsAsync(int productId, int count = 4);
        Task<ProductListViewModel> GetProductsAsync(ProductListViewModel filter);
        Task<List<Product>> GetFeaturedProductsAsync(int count = 8);
        Task<List<Product>> GetNewArrivalsAsync(int count = 8);
        Task<List<Product>> GetBestSellersAsync(int count = 8);
        Task<List<Brand>> GetBrandsAsync(int count = 6);
        Task<Product?> GetProductBySearchAsync(string searchProduct);
        Task<bool> AddReviewAsync(int productId, string userId, ReviewViewModel review);
    }
}
