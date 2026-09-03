using FinControl.Application.Exceptions;
using FinControl.Application.Services;
using FinControl.Domain.Entities;
using FinControl.Domain.Enums;
using FinControl.Domain.Interfaces;
using Moq;

namespace FinControl.Application.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<ITransacaoRepository> _transacaoRepository = new();
    private readonly Mock<IMetaMensalRepository> _metaMensalRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        _sut = new DashboardService(_transacaoRepository.Object, _metaMensalRepository.Object, _usuarioRepository.Object);
    }

    private static Categoria Categoria(int id, string nome) => new() { Id = id, Nome = nome, Cor = "#000000" };

    [Fact]
    public async Task ObterResumoMensalAsync_DeveCalcularTotaisESaldoCorretamente()
    {
        var alimentacao = Categoria(1, "Alimentação");
        var transacoes = new List<Transacao>
        {
            new() { CategoriaId = 1, Categoria = alimentacao, Tipo = TipoTransacao.Receita, Valor = 5000m },
            new() { CategoriaId = 1, Categoria = alimentacao, Tipo = TipoTransacao.Despesa, Valor = 800m },
            new() { CategoriaId = 1, Categoria = alimentacao, Tipo = TipoTransacao.Despesa, Valor = 200m },
        };
        _transacaoRepository.Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026)).ReturnsAsync(transacoes);
        _metaMensalRepository.Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026)).ReturnsAsync([]);

        var resumo = await _sut.ObterResumoMensalAsync(usuarioId: 42, mes: 8, ano: 2026);

        Assert.Equal(5000m, resumo.TotalReceitas);
        Assert.Equal(1000m, resumo.TotalDespesas);
        Assert.Equal(4000m, resumo.Saldo);
        Assert.Single(resumo.GastosPorCategoria);
        Assert.Equal(1000m, resumo.GastosPorCategoria[0].TotalGasto);
    }

    [Fact]
    public async Task ObterResumoMensalAsync_QuandoGastoUltrapassaMeta_DeveMarcarEstourouMeta()
    {
        var carro = Categoria(2, "Carro");
        var transacoes = new List<Transacao>
        {
            new() { CategoriaId = 2, Categoria = carro, Tipo = TipoTransacao.Despesa, Valor = 600m },
        };
        var meta = new MetaMensal { CategoriaId = 2, ValorLimite = 400m, Mes = 8, Ano = 2026 };
        _transacaoRepository.Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026)).ReturnsAsync(transacoes);
        _metaMensalRepository.Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026)).ReturnsAsync([meta]);

        var resumo = await _sut.ObterResumoMensalAsync(usuarioId: 42, mes: 8, ano: 2026);

        var gastoCarro = Assert.Single(resumo.GastosPorCategoria);
        Assert.True(gastoCarro.EstourouMeta);
        Assert.Equal(400m, gastoCarro.LimiteMeta);
    }

    [Fact]
    public async Task ObterResumoMensalAsync_QuandoGastoDentroDaMeta_NaoDeveMarcarEstourouMeta()
    {
        var carro = Categoria(2, "Carro");
        var transacoes = new List<Transacao>
        {
            new() { CategoriaId = 2, Categoria = carro, Tipo = TipoTransacao.Despesa, Valor = 300m },
        };
        var meta = new MetaMensal { CategoriaId = 2, ValorLimite = 400m, Mes = 8, Ano = 2026 };
        _transacaoRepository.Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026)).ReturnsAsync(transacoes);
        _metaMensalRepository.Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026)).ReturnsAsync([meta]);

        var resumo = await _sut.ObterResumoMensalAsync(usuarioId: 42, mes: 8, ano: 2026);

        Assert.False(resumo.GastosPorCategoria[0].EstourouMeta);
    }

    [Fact]
    public async Task ObterMediasPorCategoriaAsync_DeveDividirTotalPelaQuantidadeDeMeses()
    {
        var alimentacao = Categoria(1, "Alimentação");
        var transacoes = new List<Transacao>
        {
            new() { CategoriaId = 1, Categoria = alimentacao, Tipo = TipoTransacao.Despesa, Valor = 600m },
            new() { CategoriaId = 1, Categoria = alimentacao, Tipo = TipoTransacao.Despesa, Valor = 300m },
        };
        _transacaoRepository
            .Setup(r => r.GetByUsuarioDesdeAsync(42, It.IsAny<DateOnly>()))
            .ReturnsAsync(transacoes);

        var medias = (await _sut.ObterMediasPorCategoriaAsync(usuarioId: 42, quantidadeMeses: 3)).ToList();

        var media = Assert.Single(medias);
        Assert.Equal(300m, media.MediaMensal);
        Assert.Equal(3, media.MesesConsiderados);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ObterMediasPorCategoriaAsync_QuandoQuantidadeMesesInvalida_DeveLancarArgumentOutOfRangeException(
        int quantidadeMeses)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.ObterMediasPorCategoriaAsync(usuarioId: 42, quantidadeMeses));
    }

    [Fact]
    public async Task ObterSugestaoInvestimentoAsync_DeveCalcularValorSugeridoCorretamente()
    {
        var usuario = new Usuario { Id = 42, RendaMensal = 6000m };
        _usuarioRepository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(usuario);
        _transacaoRepository
            .Setup(r => r.GetByUsuarioEPeriodoAsync(42, 8, 2026))
            .ReturnsAsync([new Transacao { Tipo = TipoTransacao.Despesa, Valor = 3580.5m }]);

        var sugestao = await _sut.ObterSugestaoInvestimentoAsync(usuarioId: 42, mes: 8, ano: 2026, percentualReserva: 0.20m);

        Assert.Equal(6000m, sugestao.RendaMensal);
        Assert.Equal(3580.5m, sugestao.TotalDespesasMes);
        Assert.Equal(1200m, sugestao.ValorReservaSeguranca);
        Assert.Equal(1219.5m, sugestao.ValorSugeridoInvestimento);
    }

    [Fact]
    public async Task ObterSugestaoInvestimentoAsync_QuandoUsuarioNaoExiste_DeveLancarNotFoundException()
    {
        _usuarioRepository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.ObterSugestaoInvestimentoAsync(usuarioId: 42, mes: 8, ano: 2026));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public async Task ObterSugestaoInvestimentoAsync_QuandoPercentualForaDoIntervalo_DeveLancarArgumentOutOfRangeException(
        decimal percentualReserva)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.ObterSugestaoInvestimentoAsync(usuarioId: 42, mes: 8, ano: 2026, percentualReserva));
    }
}
