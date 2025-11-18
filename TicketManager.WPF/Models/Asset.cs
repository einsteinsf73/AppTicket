using System;

namespace TicketManager.WPF.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Item { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string AssetNumber { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Warranty { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Employee { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Manutencao { get; set; } = string.Empty;
        public DateTime? PrevisaoManutencao { get; set; }
        public string? Observations { get; set; } = string.Empty;
    }
}