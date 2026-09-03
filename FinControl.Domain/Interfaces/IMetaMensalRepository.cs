using FinControl.Domain.Entities;

namespace FinControl.Domain.Interfaces;

public interface IMetaMensalRepository : IRepository<MetaMensal>
{
    Task<IEnumerable<MetaMensal>> GetByUsuarioEPeriodoAsync(int usuarioId, int mes, int ano);
}
