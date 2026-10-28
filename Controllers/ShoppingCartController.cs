using Microsoft.AspNetCore.Mvc;
using MobileShop.Data;
using MobileShop.IServices;

namespace MobileShop.Controllers
{
    public class ShoppingCartController(IShoppingCartService shoppingCardService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var card = await shoppingCardService.GetCartAsync();
            ViewBag.CartItemCount = card.ItemCount;
            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            await shoppingCardService.AddToCartAsync(productId, quantity);

            var itemCount = await shoppingCardService.GetCartItemCountAsync();

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new { success = true, itemCount, message = "Product added to cart!" });
            }
            TempData["Success"] = "Product added to cart!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            await shoppingCardService.UpdateCartItemAsync(cartItemId, quantity);
            var cart = await shoppingCardService.GetCartAsync();
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    itemCount = cart.ItemCount,
                    cartTotal = cart.CartTotal,
                    grandTotal = cart.GrandTotal,
                    tax = cart.Tax,
                    shipping = cart.Shipping
                });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            await shoppingCardService.RemoveFromCartAsync(cartItemId);
            var cart = await shoppingCardService.GetCartAsync();

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    itemCount = cart.ItemCount,
                    cartTotal = cart.CartTotal,
                    grandTotal = cart.GrandTotal
                });
            }

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            await shoppingCardService.ClearCartAsync();
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new { success = true, itemCount = 0 });
            }

            TempData["Success"] = "Cart cleared.";
            return RedirectToAction(nameof(Index));
        }

    }
}
