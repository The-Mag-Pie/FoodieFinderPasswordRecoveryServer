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

            if (Helpers.IsExpired(PasswordRecoveryData.CreatedEpoch))
            {
                _dbContext.PasswordRecovery.Remove(PasswordRecoveryData);
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status403Forbidden, "Password recovery request expired");
            }

            return Page();
        }
    }
}
