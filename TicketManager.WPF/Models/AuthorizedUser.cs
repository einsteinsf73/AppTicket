
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

        [Column("IS_ADMIN")]
        public int IsAdmin { get; set; }

        [Column("IS_ACTIVE")]
        public int IsActive { get; set; } = 1; // Default to active

        [Column("HAS_TICKET_ACCESS")]
        public int HasTicketAccess { get; set; } = 1; // Default to true

        [Column("HAS_ASSET_ACCESS")]
        public int HasAssetAccess { get; set; } = 1; // Default to true

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

        [NotMapped]
        public bool HasTicketAccessBool
        {
            get { return HasTicketAccess == 1; }
            set { HasTicketAccess = value ? 1 : 0; }
        }

        [NotMapped]
        public bool HasAssetAccessBool
        {
            get { return HasAssetAccess == 1; }
            set { HasAssetAccess = value ? 1 : 0; }
        }
    }
}
