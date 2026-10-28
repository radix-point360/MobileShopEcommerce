using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;

namespace MobileShop.ViewComponents
{
    public class BrandsMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public BrandsMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();

            return View(brands);
        }
    }
}
