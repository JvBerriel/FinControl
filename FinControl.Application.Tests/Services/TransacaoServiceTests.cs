using FinControl.Application.DTOs;
using FinControl.Application.Exceptions;
using FinControl.Application.Services;
using FinControl.Domain.Entities;
using FinControl.Domain.Enums;
using FinControl.Domain.Interfaces;
using Moq;

namespace FinControl.Application.Tests.Services;

public class TransacaoServiceTests
{
    private readonly Mock<ITransacaoRepository> _transacaoRepository = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly TransacaoService _sut;

    public TransacaoServiceTests()
    {
        _sut = new TransacaoService(_transacaoRepository.Object, _categoriaRepository.Object, _unitOfWork.Object);
    }

    private static Categoria CategoriaDoUsuario(int categoriaId, int usuarioId) =>
        new() { Id = categoriaId, UsuarioId = usuarioId, Nome = "Alimentação", Cor = "#eb6834" };

    [Fact]
    public async Task CriarAsync_QuandoCategoriaNaoPertenceAoUsuario_DeveLancarNotFoundException()
    {
        _categoriaRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(CategoriaDoUsuario(5, usuarioId: 99));
        var dto = new CriarTransacaoDto(5, "Mercado", 100m, TipoTransacao.Despesa, new DateOnly(2026, 8, 1));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CriarAsync(usuarioId: 42, dto));

        _transacaoRepository.Verify(r => r.AddAsync(It.IsAny<Transacao>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_QuandoCategoriaExisteEPertenceAoUsuario_DeveCriarTransacao()
    {
        _categoriaRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(CategoriaDoUsuario(5, usuarioId: 42));
        var dto = new CriarTransacaoDto(5, "Mercado", 100m, TipoTransacao.Despesa, new DateOnly(2026, 8, 1));

        Transacao? transacaoCriada = null;
        _transacaoRepository
            .Setup(r => r.AddAsync(It.IsAny<Transacao>()))
            .Callback<Transacao>(t => transacaoCriada = t)
            .Returns(Task.CompletedTask);
        _transacaoRepository
            .Setup(r => r.GetByIdComCategoriaAsync(It.IsAny<int>()))
            .ReturnsAsync(() => transacaoCriada is null ? null : new Transacao
            {
                Id = transacaoCriada.Id,
                UsuarioId = transacaoCriada.UsuarioId,
                CategoriaId = transacaoCriada.CategoriaId,
                Categoria = CategoriaDoUsuario(5, 42),
                Descricao = transacaoCriada.Descricao,
                Valor = transacaoCriada.Valor,
                Tipo = transacaoCriada.Tipo,
                DataTransacao = transacaoCriada.DataTransacao,
            });

        var resultado = await _sut.CriarAsync(usuarioId: 42, dto);

        Assert.Equal(42, resultado.UsuarioId);
        Assert.Equal("Mercado", resultado.Descricao);
        Assert.Equal("Alimentação", resultado.CategoriaNome);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoTransacaoPertenceAOutroUsuario_DeveLancarNotFoundException()
    {
        var transacaoDeOutroUsuario = new Transacao { Id = 1, UsuarioId = 99, CategoriaId = 5 };
        _transacaoRepository.Setup(r => r.GetByIdComCategoriaAsync(1)).ReturnsAsync(transacaoDeOutroUsuario);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPorIdAsync(id: 1, usuarioId: 42));
    }

    [Fact]
    public async Task AtualizarAsync_QuandoNovaCategoriaNaoPertenceAoUsuario_DeveLancarNotFoundException()
    {
        var transacaoDoUsuario = new Transacao { Id = 1, UsuarioId = 42, CategoriaId = 5 };
        _transacaoRepository.Setup(r => r.GetByIdComCategoriaAsync(1)).ReturnsAsync(transacaoDoUsuario);
        _categoriaRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(CategoriaDoUsuario(7, usuarioId: 99));

        var dto = new AtualizarTransacaoDto(7, "Nova descrição", 50m, TipoTransacao.Despesa, new DateOnly(2026, 8, 2));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.AtualizarAsync(id: 1, usuarioId: 42, dto));
    }

    [Fact]
    public async Task RemoverAsync_QuandoTransacaoPertenceAoUsuario_DeveRemoverESalvar()
    {
        var transacao = new Transacao { Id = 1, UsuarioId = 42, CategoriaId = 5 };
        _transacaoRepository.Setup(r => r.GetByIdComCategoriaAsync(1)).ReturnsAsync(transacao);

        await _sut.RemoverAsync(id: 1, usuarioId: 42);

        _transacaoRepository.Verify(r => r.Remove(transacao), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
