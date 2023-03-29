using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieFinderPasswordRecoveryServer.Models
{
    public class User
    {
        [Column("UserId")]
        public int ID { get; set; }

        public string Email { get; set; }

        [Column("Password")]
        public string EncryptedPassword { get; set; }
    }
}
