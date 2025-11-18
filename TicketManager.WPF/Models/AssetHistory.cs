
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.WPF.Models
{
    public class AssetHistory
    {
        [Key]
        public int Id { get; set; }

        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }

        public string Item { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string AssetNumber { get; set; }
        public decimal Value { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNumber { get; set; }
        public string Warranty { get; set; }
        public string Location { get; set; }
        public string Sector { get; set; }
        public string Department { get; set; }
        public string Employee { get; set; }
        public string Status { get; set; }
        public string? Manutencao { get; set; }
        public DateTime? PrevisaoManutencao { get; set; }
        public string? Observations { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
    }
}
