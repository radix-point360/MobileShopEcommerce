using System.ComponentModel.DataAnnotations;

namespace MobileShop.ViewModel
{
    public class ShoppingCartViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        [Display(Name = "Cart Total")]
        public decimal CartTotal { get; set; }
        [Display(Name = "Item Count")]
        public int ItemCount { get; set; }

        [Display(Name = "Tax")]
        public decimal Tax { get; set; }

        [Display(Name = "Shipping")]
        public decimal Shipping { get; set; }

        public decimal GrandTotal => CartTotal + Tax + Shipping;

    }
}
