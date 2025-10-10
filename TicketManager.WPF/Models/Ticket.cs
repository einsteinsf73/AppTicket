
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace TicketManager.WPF.Models
{
    public enum TicketStatus
    {
        Aberto,
        EmAndamento,
        Resolvido,
        Fechado
    }

    public enum TicketPriority
    {
        Baixa,
        Media,
        Alta
    }

    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Campos de Auditoria de Criação
        [MaxLength(256)]
        public string CreatedByWindowsUser { get; set; } = string.Empty;

        [MaxLength(256)]
        public string CreatedByHostname { get; set; } = string.Empty;
        public int SlaMinutes { get; set; }
        public virtual ICollection<ReopeningLog> ReopeningLogs { get; set; } = new List<ReopeningLog>();
    }
}
