using System;

namespace TicketManager.WPF.Models
{
    public class ReopeningLog
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public DateTime ReopenedAt { get; set; }
        public string ReopenedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;

        public virtual Ticket Ticket { get; set; } = null!;
    }
}
