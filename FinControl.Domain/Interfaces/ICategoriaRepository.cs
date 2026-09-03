using FinControl.Domain.Entities;

namespace FinControl.Domain.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<IEnumerable<Categoria>> GetByUsuarioIdAsync(int usuarioId);
}
