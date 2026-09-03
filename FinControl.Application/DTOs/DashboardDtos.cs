namespace FinControl.Application.DTOs;

public record GastoPorCategoriaDto(int CategoriaId, string CategoriaNome, decimal TotalGasto, decimal? LimiteMeta, bool EstourouMeta);

public record ResumoMensalDto(
    int Mes,
    int Ano,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo,
    IReadOnlyList<GastoPorCategoriaDto> GastosPorCategoria);

public record MediaCategoriaDto(int CategoriaId, string CategoriaNome, decimal MediaMensal, int MesesConsiderados);

public record SugestaoInvestimentoDto(
    decimal RendaMensal,
    decimal TotalDespesasMes,
    decimal PercentualReserva,
    decimal ValorReservaSeguranca,
    decimal ValorSugeridoInvestimento);
