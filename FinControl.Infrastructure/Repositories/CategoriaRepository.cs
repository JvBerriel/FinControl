using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using FinControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinControl.Infrastructure.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(FinControlDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Categoria>> GetByUsuarioIdAsync(int usuarioId) =>
        await DbSet.Where(c => c.UsuarioId == usuarioId).ToListAsync();
}
