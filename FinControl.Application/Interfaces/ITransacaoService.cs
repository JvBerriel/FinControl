using FinControl.Application.DTOs;

namespace FinControl.Application.Interfaces;

public interface ITransacaoService
{
    Task<IEnumerable<TransacaoDto>> ListarPorUsuarioAsync(int usuarioId);
    Task<TransacaoDto> ObterPorIdAsync(int id, int usuarioId);
    Task<TransacaoDto> CriarAsync(int usuarioId, CriarTransacaoDto dto);
    Task<TransacaoDto> AtualizarAsync(int id, int usuarioId, AtualizarTransacaoDto dto);
    Task RemoverAsync(int id, int usuarioId);
}
