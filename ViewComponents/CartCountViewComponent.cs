using Microsoft.AspNetCore.Mvc;
using MobileShop.IServices;

namespace MobileShop.ViewComponents
{
    public class CartCountViewComponent(IShoppingCartService cartService): ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await cartService.GetCartItemCountAsync();

            return View(count);
        }
    }
}
