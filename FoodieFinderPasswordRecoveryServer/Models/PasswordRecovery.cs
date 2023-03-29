using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieFinderPasswordRecoveryServer.Models
{
    public class PasswordRecovery
    {
        [Key]
        public string UUID { get; set; }
        public long CreatedEpoch { get; set; }

        [Column("User_UserId")]
        public int UserID { get; set; }
    }
}
