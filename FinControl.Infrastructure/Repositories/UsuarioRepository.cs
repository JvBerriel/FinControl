using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using FinControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinControl.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(FinControlDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await DbSet.FirstOrDefaultAsync(u => u.Email == email);
}
