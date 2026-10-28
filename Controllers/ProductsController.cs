using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MobileShop.IServices;
using MobileShop.Models;
using MobileShop.ViewModel;
using Stripe;

namespace MobileShop.Controllers
{
    public class ProductsController(IProductService productService, UserManager<ApplicationUser> userManager, IShoppingCartService cartService) : Controller
    {
        public async Task<IActionResult> Index(int? categoryId, int? brandId, string? search, string? sort, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            var filter = new ProductListViewModel
            {
                SelectedCategoryId = categoryId,
                SelectedBrandId = brandId,
                SearchTerm = search,
                SortOrder = sort,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                PageNumber = page
            };

            var result = await productService.GetProductsAsync(filter);

            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var getProduct = await productService.GetProductByIdAsync(id);

            if (getProduct is null)
                return NotFound();

            var getRelatedProduct = await productService.GetRelatedProductsAsync(id);

            bool isInWishlist = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                //var user = await userManager.GetUserAsync(User);
                var user = await productService.GetProductByIdAsync(id);
                isInWishlist = user?.WishlistItems.Any(w => w.ProductId == id) ?? false;
            }

            var productDetails = new ProductDetailViewModel
            {
                Product = getProduct,
                RelatedProducts = getRelatedProduct,
                IsInWishlist = isInWishlist

            };

            return View(productDetails);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return Json(new List<object>());
            }

            var searchFilter = new ProductListViewModel
            {
                SearchTerm = searchTerm,
                PageSize = 5
            };

            var result = await productService.GetProductsAsync(searchFilter);

            var suggestions = result.products.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                price = p.SalePrice,
                image = p.MainImageUrl,
                brand = p.Brand.Name
            });

            return Json(suggestions);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await productService.AddReviewAsync(productId, user.Id, model);
            if (result)
            {
                TempData["Success"] = "Thank you for your review!";
            }
            else
            {
                TempData["Error"] = "Failed to add review. Please try again.";
            }

            return RedirectToAction(nameof(Details), new { id = productId });
        }
        public async Task<IActionResult> Compare(int[] ids)
        {
            if (ids == null || ids.Length < 2)
            {
                TempData["Error"] = "Please select at least 2 products to compare.";
                return RedirectToAction(nameof(Index));
            }

            var products = new List<Models.Product>();

            foreach (var id in ids.Distinct().Take(4))
            {
                var product = await productService.GetProductByIdAsync(id);

                if (product != null)
                {
                    products.Add(product);
                }
            }

            //if (products.Count < 2)
            //{
            //    TempData["Error"] = "Compare requires minimum 2 valid products.";
            //    return RedirectToAction(nameof(Index));
            //}

            var specificationNames = products
                .SelectMany(p => p.Specifications)
                .Select(s => s.Name)
                .Distinct()
                .ToList();

            var viewModel = new CompareProductsViewModel
            {
                Products = products,
                SpecificationNames = specificationNames
            };

            ViewBag.CartItemCount = await cartService.GetCartItemCountAsync();

            return View(viewModel);
        }
    }
}


