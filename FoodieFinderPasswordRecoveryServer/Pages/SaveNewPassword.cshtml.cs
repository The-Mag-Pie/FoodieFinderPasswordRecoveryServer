using FoodieFinderPasswordRecoveryServer.Database;
using FoodieFinderPasswordRecoveryServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace FoodieFinderPasswordRecoveryServer.Pages
{
    public class SaveNewPasswordModel : PageModel
    {
        private readonly AppDbContext _dbContext;

        public SaveNewPasswordModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult OnPost(string newPassword, string uuid)
        {
            PasswordRecovery passwordRecoveryData;
            User userData;

            if (newPassword == null || uuid == null ||
                newPassword.Length == 0 || uuid.Length == 0)
            {
                return BadRequest("Invalid parameters");
            }

            try
            {
                passwordRecoveryData = _dbContext.PasswordRecovery.Where(p => p.UUID == uuid).Single();
                userData = _dbContext.User.Where(u => u.ID == passwordRecoveryData.UserID).Single();
            }
            catch
            {
                return NotFound("User or password recovery request not found");
            }

            if (userData.EncryptedPassword == "auth0")
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Cannot change password for user that is using Auth0 authentication method");
            }

            if (Helpers.IsExpired(passwordRecoveryData.CreatedEpoch))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Password recovery request expired");
            }

            userData.EncryptedPassword = EncryptPassword(newPassword);
            _dbContext.User.Update(userData);
            _dbContext.PasswordRecovery.Remove(passwordRecoveryData);
            _dbContext.SaveChanges();

            return Redirect("/PasswordChanged");
        }

        private static string EncryptPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
