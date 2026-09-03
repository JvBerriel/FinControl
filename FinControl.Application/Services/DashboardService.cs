using FinControl.Application.DTOs;
using FinControl.Application.Exceptions;
using FinControl.Application.Interfaces;
using FinControl.Domain.Enums;
using FinControl.Domain.Interfaces;

namespace FinControl.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IMetaMensalRepository _metaMensalRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public DashboardService(
        ITransacaoRepository transacaoRepository,
        IMetaMensalRepository metaMensalRepository,
        IUsuarioRepository usuarioRepository)
    {
        _transacaoRepository = transacaoRepository;
        _metaMensalRepository = metaMensalRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ResumoMensalDto> ObterResumoMensalAsync(int usuarioId, int mes, int ano)
    {
        var transacoes = (await _transacaoRepository.GetByUsuarioEPeriodoAsync(usuarioId, mes, ano)).ToList();
        var metas = (await _metaMensalRepository.GetByUsuarioEPeriodoAsync(usuarioId, mes, ano))
            .ToDictionary(m => m.CategoriaId, m => m.ValorLimite);

        var totalReceitas = transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
        var totalDespesas = transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

        var gastosPorCategoria = transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .GroupBy(t => new { t.CategoriaId, CategoriaNome = t.Categoria?.Nome ?? string.Empty })
            .Select(g =>
            {
                var limite = metas.TryGetValue(g.Key.CategoriaId, out var valorLimite) ? valorLimite : (decimal?)null;
                var totalGasto = g.Sum(t => t.Valor);
                return new GastoPorCategoriaDto(
                    g.Key.CategoriaId,
                    g.Key.CategoriaNome,
                    totalGasto,
                    limite,
                    limite.HasValue && totalGasto > limite.Value);
            })
            .OrderByDescending(g => g.TotalGasto)
            .ToList();

        return new ResumoMensalDto(mes, ano, totalReceitas, totalDespesas, totalReceitas - totalDespesas, gastosPorCategoria);
    }

    public async Task<IEnumerable<MediaCategoriaDto>> ObterMediasPorCategoriaAsync(int usuarioId, int quantidadeMeses = 6)
    {
        if (quantidadeMeses <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidadeMeses), "A quantidade de meses deve ser maior que zero.");

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var primeiroDiaJanela = new DateOnly(hoje.Year, hoje.Month, 1).AddMonths(-(quantidadeMeses - 1));

        var transacoes = await _transacaoRepository.GetByUsuarioDesdeAsync(usuarioId, primeiroDiaJanela);

        return transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .GroupBy(t => new { t.CategoriaId, CategoriaNome = t.Categoria?.Nome ?? string.Empty })
            .Select(g => new MediaCategoriaDto(
                g.Key.CategoriaId,
                g.Key.CategoriaNome,
                Math.Round(g.Sum(t => t.Valor) / quantidadeMeses, 2),
                quantidadeMeses))
            .OrderByDescending(m => m.MediaMensal)
            .ToList();
    }

    public async Task<SugestaoInvestimentoDto> ObterSugestaoInvestimentoAsync(
        int usuarioId, int mes, int ano, decimal percentualReserva = 0.20m)
    {
        if (percentualReserva is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(percentualReserva), "O percentual de reserva deve estar entre 0 e 1.");

        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
            ?? throw new NotFoundException($"Usuário {usuarioId} não encontrado.");

        var transacoes = await _transacaoRepository.GetByUsuarioEPeriodoAsync(usuarioId, mes, ano);
        var totalDespesas = transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

        var valorReserva = Math.Round(usuario.RendaMensal * percentualReserva, 2);
        var valorSugerido = usuario.RendaMensal - totalDespesas - valorReserva;

        return new SugestaoInvestimentoDto(
            usuario.RendaMensal,
            totalDespesas,
            percentualReserva,
            valorReserva,
            valorSugerido);
    }
}
