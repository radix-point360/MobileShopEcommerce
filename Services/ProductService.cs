using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.IServices;
using MobileShop.Models;
using MobileShop.ViewModel;
using Stripe.Reporting;
using System.CodeDom.Compiler;
namespace MobileShop.Services
{
    public class ProductService(ApplicationDbContext context) : IProductService
    {
        //private readonly ApplicationDbContext _context;
        //public ProductService(ApplicationDbContext context)
        //{
        //    _context = context;

        //}
        public async Task<bool> AddReviewAsync(int productId, string userId, ReviewViewModel review)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null) { return false; }

            var newReview = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = review.Rating,
                Title = review.Title,
                Comment = review.Comment,
                IsApproved = true 
            };

            context.Reviews.Add(newReview);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Product>> GetBestSellersAsync(int count = 8)
        {
            return await context.Products.Include(p => p.Brand).Include(p => p.Reviews).Where(p => p.IsBestseller && p.IsActive).Take(count).ToListAsync();
        }

        public async Task<List<Brand>> GetBrandsAsync(int count = 6)
        {
            return await context.Brands.Where(b => b.IsActive).Take(count).ToListAsync();
        }

        public async Task<List<Product>> GetFeaturedProductsAsync(int count = 8)
        {
            return await context.Products.Include(p => p.Brand).Include(p => p.WishlistItems).Include(p => p.Reviews).Where(p => p.IsFeatured && p.IsActive).Take(count).ToListAsync();
        }

        public async Task<List<Product>> GetNewArrivalsAsync(int count = 8)
        {
            return await context.Products
                .Include(p => p.Brand)
                .Include(p => p.Reviews)
                .Where(p => p.IsNewArrival && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await context.Products
               .Include(p => p.Brand)
               .Include(p => p.Category)
               .Include(p => p.ProductImages)
               .Include(p => p.Specifications)
               .Include(p => p.WishlistItems)
               .Include(p => p.Reviews)
               .ThenInclude(p => p.User)
               .FirstOrDefaultAsync(p => p.Id == productId);

        }

        public async Task<Product?> GetProductBySearchAsync(string searchProduct)
        {
            return await context.Products
               .Include(p => p.Brand)
               .Include(p => p.Category)
               .Include(p => p.ProductImages)
               .Include(p => p.Specifications)
               .Include(p => p.Reviews)
               .ThenInclude(p => p.User)
               .FirstOrDefaultAsync(p => p.Name.Contains(searchProduct) ||
               p.Brand.Name.Contains(searchProduct) || 
               p.Category.Name.Contains(searchProduct) ||
               p.Specifications.Any(a => a.Name.Contains(searchProduct) || a.Value.Contains(searchProduct)));
        }

        public async Task<ProductListViewModel> GetProductsAsync(ProductListViewModel filter)
        {
            //AsQueryable() converts a collection into an IQueryable, allowing to keep building the query dynamically before it is executed.
            //Initialize quer
            var query = context.Products.Include(p => p.Category).Include(p => p.Brand).Include(p => p.Reviews).Where(p => p.IsActive).AsQueryable();

            if (filter.SelectedCategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.SelectedCategoryId.Value);

            if (filter.SelectedBrandId.HasValue)
                query = query.Where(p => p.BrandId == filter.SelectedBrandId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(p => p.Name.Contains(filter.SearchTerm) ||
                                         p.Description.Contains(filter.SearchTerm) ||
                                         p.Brand.Name.Contains(filter.SearchTerm));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.SalePrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.SalePrice <= filter.MaxPrice.Value);

            query = filter.SortOrder?.ToLower()
                switch
            {
                "price_asc" => query.OrderBy(p => p.SalePrice),
                "price_desc" => query.OrderByDescending(p => p.SalePrice),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            filter.TotalCount = await query.CountAsync();
            filter.TotalPages = (int)Math.Ceiling(filter.TotalCount / (double)filter.PageSize);

            // 5. Execute Query with Skip/Take
            filter.products = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // 6. Populate filter dropdowns
            filter.Categories = await context.Categories.Where(c => c.IsActive).ToListAsync();
            filter.Brands = await context.Brands.Where(b => b.IsActive).ToListAsync();

            return filter;

        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId, int count = 4)
        {
           var product = await context.Products.Where(p => p.Id == productId && p.IsActive).FirstOrDefaultAsync();

            if (product is null)
                new List<Product>();

            var relatedProducts = await context.Products
                .Include(p => p.Brand)
                .Where(p => p.Id != productId && p.IsActive && (p.BrandId == product.BrandId || p.CategoryId == product.CategoryId))
                .OrderBy(p => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            return relatedProducts;
        }
    }
}
