using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using GEntretien.Infrastructure.Persistence;

namespace GEntretien.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _db;

        public EquipmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Equipment equipment)
        {
            _db.Equipments.Add(equipment);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _db.Equipments.FindAsync(id);
            if (e is null) return;
            _db.Equipments.Remove(e);
            await _db.SaveChangesAsync();
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _db.Equipments.FindAsync(id);
        }

        public async Task<List<Equipment>> ListAsync()
        {
            return await _db.Equipments.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            _db.Equipments.Update(equipment);
            await _db.SaveChangesAsync();
        }
    }
}
