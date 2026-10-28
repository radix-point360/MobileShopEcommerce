using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MobileShop.Data;
using MobileShop.IServices;
using MobileShop.Models;
using MobileShop.ViewModel;

namespace MobileShop.Services
{
    public class ShoppingCartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) : IShoppingCartService
    {

        private string GetCartId()
        {
            //httpContextAccessor allows to access the current HttpContext from classes where HttpContext is not directly available.
            var httpContext = httpContextAccessor.HttpContext;

            var cartId = httpContext?.Session.GetString("CartId");

            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                httpContext?.Session.SetString("CartId", cartId);
            }

            return cartId;
        }


        public async Task AddToCartAsync(int productId, int quantity = 1)
        {
            var httpContext = httpContextAccessor.HttpContext;

            var cartId = httpContext?.User.Identity?.IsAuthenticated == true ? userManager.GetUserId(httpContext.User): GetCartId();

            var cartItem = await dbContext.ShoppingCartItems.FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = cartItem.Quantity + quantity;
            }
            else
            {
                cartItem = new ShoppingCartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                dbContext.ShoppingCartItems.Add(cartItem);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task ClearCartAsync()
        {
            var cartId = "";

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                cartId = userManager.GetUserId(httpContext.User);
            }
            else
            {
                cartId = GetCartId();
            }

            var getCardItem = await dbContext.ShoppingCartItems.Where(s => s.CartId == cartId).ToListAsync();

            if (getCardItem != null)
            {
                dbContext.ShoppingCartItems.RemoveRange(getCardItem);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCartViewModel> GetCartAsync()
        {
            var cartId = "";

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                cartId = userManager.GetUserId(httpContext.User);
            }
            else
            {
                cartId = GetCartId();
            }

            var cartItems = await dbContext.ShoppingCartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cartItems.Select(c => new CartItemViewModel
                {
                    CartItemId = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductImage = c.Product.MainImageUrl,
                    UnitPrice = c.Product.SalePrice,
                    Quantity = c.Quantity,
                    StockQuantity = c.Product.StockQuantity
                }).ToList(),
                CartTotal = cartItems.Sum(c => c.Product.SalePrice * c.Quantity),
                ItemCount = cartItems.Sum(c => c.Quantity),
                Tax = cartItems.Sum(c => c.Product.SalePrice * c.Quantity) * 0.18m,
                Shipping = cartItems.Sum(c => c.Product.SalePrice * c.Quantity) > 50000 ? 0 : 500
            };

            return viewModel;
        }

        public async Task<int> GetCartItemCountAsync()
        {
            var cartId = "";

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                cartId = userManager.GetUserId(httpContext.User);
            }
            else
            {
                cartId = GetCartId();
            }
            return await dbContext.ShoppingCartItems
                        .Where(c => c.CartId == cartId)
                        .SumAsync(c => c.Quantity);
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartId = "";

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                cartId = userManager.GetUserId(httpContext.User);
            }
            else
            {
                cartId = GetCartId();
            }

            var getCardItem = await dbContext.ShoppingCartItems.FirstOrDefaultAsync(s => s.CartId == cartId && s.Id == cartItemId);

            if (getCardItem != null)
            {
                dbContext.ShoppingCartItems.Remove(getCardItem);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var cartId = "";

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                cartId = userManager.GetUserId(httpContext.User);
            }
            else
            {
                cartId = GetCartId();
            }

            var CardItem = await dbContext.ShoppingCartItems.FirstOrDefaultAsync(s => s.Id == cartItemId && s.CartId == cartId);
            if (CardItem != null)
            {
                if (quantity <= 0)
                {
                    dbContext.ShoppingCartItems.Remove(CardItem);
                }
                else
                {
                    CardItem.Quantity = quantity;
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task MigrateCartAsync(string userId)
        {
            var cartId = GetCartId();
            var cartItems = await dbContext.ShoppingCartItems
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.CartId = userId;
            }

            await dbContext.SaveChangesAsync();
            httpContextAccessor.HttpContext?.Session.Remove("CartId");
        }
    }
}
