using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using FinControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinControl.Infrastructure.Repositories;

public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(FinControlDbContext context) : base(context)
    {
    }

    public async Task<Transacao?> GetByIdComCategoriaAsync(int id) =>
        await DbSet.Include(t => t.Categoria).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Transacao>> GetByUsuarioIdAsync(int usuarioId) =>
        await DbSet.Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.DataTransacao)
            .ToListAsync();

    public async Task<IEnumerable<Transacao>> GetByUsuarioDesdeAsync(int usuarioId, DateOnly dataInicio) =>
        await DbSet.Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId && t.DataTransacao >= dataInicio)
            .ToListAsync();

    public async Task<IEnumerable<Transacao>> GetByUsuarioEPeriodoAsync(int usuarioId, int mes, int ano) =>
        await DbSet.Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId
                && t.DataTransacao.Month == mes
                && t.DataTransacao.Year == ano)
            .OrderByDescending(t => t.DataTransacao)
            .ToListAsync();
}
