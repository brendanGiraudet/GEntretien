using System;

namespace GEntretien.Domain.Entities
{
    public class Intervention
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Equipment? Equipment { get; set; }
    }
}
