using FoodieFinderPasswordRecoveryServer.Database;
using FoodieFinderPasswordRecoveryServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodieFinderPasswordRecoveryServer.Pages
{
    public class NewPasswordModel : PageModel
    {
        private readonly AppDbContext _dbContext;

        public PasswordRecovery PasswordRecoveryData { get; set; }
        public User UserData { get; set; }

        public NewPasswordModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult OnGet(string uuid)
        {
            try
            {
                PasswordRecoveryData = _dbContext.PasswordRecovery.Where(p => p.UUID == uuid).Single();
                UserData = _dbContext.User.Where(u => u.ID == PasswordRecoveryData.UserID).Single();
            }
            catch
            {
                return NotFound("User or password recovery request not found");
            }

            if (UserData.EncryptedPassword == "auth0")
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Cannot change password for user that is using Auth0 authentication method");
            }

            if (Helpers.IsExpired(PasswordRecoveryData.CreatedEpoch))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Password recovery request expired");
            }

            return Page();
        }
    }
}
