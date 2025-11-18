using System;

namespace TicketManager.WPF.Models
{
    public class TicketLog
    {
        public int Id { get; set; }

        // Dados do ticket original
        public int OriginalTicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public int SlaMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Dados do Log
        public string DeletedByHostname { get; set; } = string.Empty;
        public string DeletedByWindowsUser { get; set; } = string.Empty;
        public DateTime DeletionTimestamp { get; set; }
    }
}
