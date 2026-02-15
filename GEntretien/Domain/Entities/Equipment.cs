using System;
using System.Collections.Generic;

namespace GEntretien.Domain.Entities
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Location { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public List<Intervention> Interventions { get; set; } = new();
    }
}
