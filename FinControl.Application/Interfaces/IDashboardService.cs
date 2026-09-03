using FinControl.Application.DTOs;

namespace FinControl.Application.Interfaces;

public interface IDashboardService
{
    Task<ResumoMensalDto> ObterResumoMensalAsync(int usuarioId, int mes, int ano);
    Task<IEnumerable<MediaCategoriaDto>> ObterMediasPorCategoriaAsync(int usuarioId, int quantidadeMeses = 6);
    Task<SugestaoInvestimentoDto> ObterSugestaoInvestimentoAsync(int usuarioId, int mes, int ano, decimal percentualReserva = 0.20m);
}
