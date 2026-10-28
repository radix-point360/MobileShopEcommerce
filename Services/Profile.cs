using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using MobileShop.IServices;
using MobileShop.Models;
using MobileShop.ViewModel;
using Stripe;

namespace MobileShop.Services
{
    public class Profile(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IFileService fileService) : IProfile
    {
        public async Task<ApplicationUser?> GetLoggedInUserProfileInfoAsync(ProfileViewModel? model = null)
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext?.User);

            if (user == null)
                return null;

            user.FirstName = string.IsNullOrWhiteSpace(model.FirstName) ? user.FirstName : model.FirstName;
            user.LastName = string.IsNullOrWhiteSpace(model.LastName) ? user.LastName : model.LastName;
            user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? user.PhoneNumber : model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth.HasValue ? model.DateOfBirth : user.DateOfBirth;
            user.Address = string.IsNullOrWhiteSpace(model.Address) ? user.Address : model.Address;
            user.City = string.IsNullOrWhiteSpace(model.City) ? user.City : model.City;
            user.PostalCode = string.IsNullOrWhiteSpace(model.PostalCode) ? user.PostalCode : model.PostalCode;
            user.Country = string.IsNullOrWhiteSpace(model.Country) ? user.Country : model.Country;

            if (model.ProfileImage != null)
            {
                var imagePath = await fileService.SaveFileAsync(model.ProfileImage, "images/profiles");
                if (!string.IsNullOrEmpty(imagePath))
                {
                    if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                        fileService.DeleteFile(user.ProfileImageUrl);
                    user.ProfileImageUrl = imagePath;
                }
            }
            else
            {
                user.ProfileImageUrl = user.ProfileImageUrl;
            }
            return (user);
        }
    }
}