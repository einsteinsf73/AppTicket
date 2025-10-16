
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.WPF.Models
{
    [Table("AUTHORIZED_USERS")]
    public class AuthorizedUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string WindowsUserName { get; set; } = string.Empty;

        public int IsAdmin { get; set; }

        public int IsActive { get; set; } = 1; // Default to active

        [NotMapped]
        public bool IsAdminBool
        {
            get { return IsAdmin == 1; }
            set { IsAdmin = value ? 1 : 0; }
        }

        [NotMapped]
        public bool IsInactiveBool
        {
            get { return IsActive == 0; }
            set { IsActive = value ? 0 : 1; }
        }
    }
}
