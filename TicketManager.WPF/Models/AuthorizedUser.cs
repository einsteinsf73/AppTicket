
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
    }
}
