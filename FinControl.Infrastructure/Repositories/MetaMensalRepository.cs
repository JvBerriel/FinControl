using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using FinControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinControl.Infrastructure.Repositories;

public class MetaMensalRepository : Repository<MetaMensal>, IMetaMensalRepository
{
    public MetaMensalRepository(FinControlDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MetaMensal>> GetByUsuarioEPeriodoAsync(int usuarioId, int mes, int ano) =>
        await DbSet.Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId && m.Mes == mes && m.Ano == ano)
            .ToListAsync();
}
