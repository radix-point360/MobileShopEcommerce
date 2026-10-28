using MobileShop.Models;
using MobileShop.ViewModel;

namespace MobileShop.IServices
{
    public interface IProfile
    {
        Task<ApplicationUser?> GetLoggedInUserProfileInfoAsync(ProfileViewModel? model = null);
    }
}
