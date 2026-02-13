using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using GEntretien.Infrastructure.Persistence;

namespace GEntretien.Infrastructure.Repositories
{
    public class InterventionRepository : IInterventionRepository
    {
        private readonly AppDbContext _db;

        public InterventionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Intervention intervention)
        {
            _db.Interventions.Add(intervention);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var intervention = await _db.Interventions.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (intervention is null) return;
            _db.Interventions.Remove(intervention);
            await _db.SaveChangesAsync();
        }

        public async Task<Intervention?> GetByIdAsync(int id)
        {
            return await _db.Interventions.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Intervention>> ListByEquipmentAsync(int equipmentId)
        {
            return await _db.Interventions
                .AsNoTracking()
                .Where(i => i.EquipmentId == equipmentId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task UpdateAsync(Intervention intervention)
        {
            _db.Interventions.Update(intervention);
            await _db.SaveChangesAsync();
        }
    }
}
