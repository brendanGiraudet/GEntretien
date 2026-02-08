using System.Collections.Generic;
using System.Threading.Tasks;
using GEntretien.Domain.Entities;

namespace GEntretien.Domain.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<Equipment?> GetByIdAsync(int id);
        Task<List<Equipment>> ListAsync();
        Task AddAsync(Equipment equipment);
        Task UpdateAsync(Equipment equipment);
        Task DeleteAsync(int id);
    }
}
