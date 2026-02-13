using System.Collections.Generic;
using System.Threading.Tasks;
using GEntretien.Domain.Entities;

namespace GEntretien.Domain.Interfaces
{
    public interface IInterventionRepository
    {
        Task<Intervention?> GetByIdAsync(int id);
        Task<List<Intervention>> ListByEquipmentAsync(int equipmentId);
        Task AddAsync(Intervention intervention);
        Task UpdateAsync(Intervention intervention);
        Task DeleteAsync(int id);
    }
}
