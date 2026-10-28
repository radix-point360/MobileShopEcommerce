using Microsoft.AspNetCore.Mvc;
using MobileShop.Models;
using System.Diagnostics;
using MobileShop.ViewModel;
using MobileShop.IServices;

namespace MobileShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var GetFeaturedProducts = await _productService.GetFeaturedProductsAsync(8);
            var GetBestSellers = await _productService.GetBestSellersAsync(8);
            var GetNewArrivals = await _productService.GetNewArrivalsAsync(8);
            var GetBrands = await _productService.GetBrandsAsync(6);

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = GetFeaturedProducts,
                Bestsellers = GetBestSellers,
                NewArrivals = GetNewArrivals,
                Brands = GetBrands
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
