using System;

namespace GEntretien.Domain.Entities
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Location { get; set; }
    }
}
