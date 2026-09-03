using FinControl.Domain.Entities;

namespace FinControl.Domain.Interfaces;

public interface ITransacaoRepository : IRepository<Transacao>
{
    Task<Transacao?> GetByIdComCategoriaAsync(int id);
    Task<IEnumerable<Transacao>> GetByUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<Transacao>> GetByUsuarioEPeriodoAsync(int usuarioId, int mes, int ano);
    Task<IEnumerable<Transacao>> GetByUsuarioDesdeAsync(int usuarioId, DateOnly dataInicio);
}
