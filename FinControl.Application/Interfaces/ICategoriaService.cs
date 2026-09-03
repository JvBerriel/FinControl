using FinControl.Application.DTOs;

namespace FinControl.Application.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> ListarPorUsuarioAsync(int usuarioId);
    Task<CategoriaDto> ObterPorIdAsync(int id, int usuarioId);
    Task<CategoriaDto> CriarAsync(int usuarioId, CriarCategoriaDto dto);
    Task<CategoriaDto> AtualizarAsync(int id, int usuarioId, AtualizarCategoriaDto dto);
    Task RemoverAsync(int id, int usuarioId);
}
