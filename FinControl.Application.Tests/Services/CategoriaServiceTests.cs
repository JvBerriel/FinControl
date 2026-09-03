using FinControl.Application.DTOs;
using FinControl.Application.Exceptions;
using FinControl.Application.Services;
using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using Moq;

namespace FinControl.Application.Tests.Services;

public class CategoriaServiceTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CategoriaService _sut;

    public CategoriaServiceTests()
    {
        _sut = new CategoriaService(_categoriaRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarCategoriaComUsuarioIdCorreto()
    {
        var dto = new CriarCategoriaDto("Alimentação", "#eb6834", null);

        var resultado = await _sut.CriarAsync(usuarioId: 42, dto);

        Assert.Equal(42, resultado.UsuarioId);
        Assert.Equal("Alimentação", resultado.Nome);
        Assert.True(resultado.Ativa);
        _categoriaRepository.Verify(r => r.AddAsync(It.Is<Categoria>(c => c.UsuarioId == 42)), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoCategoriaNaoExiste_DeveLancarNotFoundException()
    {
        _categoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Categoria?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPorIdAsync(id: 1, usuarioId: 42));
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoCategoriaPertenceAOutroUsuario_DeveLancarNotFoundException()
    {
        var categoriaDeOutroUsuario = new Categoria { Id = 1, UsuarioId = 99, Nome = "Carro", Cor = "#2a78d6" };
        _categoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(categoriaDeOutroUsuario);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPorIdAsync(id: 1, usuarioId: 42));
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarCamposDaCategoria()
    {
        var categoria = new Categoria { Id = 1, UsuarioId = 42, Nome = "Antigo", Cor = "#000000", Ativa = true };
        _categoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(categoria);
        var dto = new AtualizarCategoriaDto("Novo Nome", "#ffffff", "casa", false);

        var resultado = await _sut.AtualizarAsync(id: 1, usuarioId: 42, dto);

        Assert.Equal("Novo Nome", resultado.Nome);
        Assert.Equal("#ffffff", resultado.Cor);
        Assert.Equal("casa", resultado.Icone);
        Assert.False(resultado.Ativa);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_QuandoCategoriaPertenceAOutroUsuario_DeveLancarNotFoundException()
    {
        var categoriaDeOutroUsuario = new Categoria { Id = 1, UsuarioId = 99, Nome = "Carro", Cor = "#2a78d6" };
        _categoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(categoriaDeOutroUsuario);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.RemoverAsync(id: 1, usuarioId: 42));

        _categoriaRepository.Verify(r => r.Remove(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_QuandoCategoriaPertenceAoUsuario_DeveRemoverESalvar()
    {
        var categoria = new Categoria { Id = 1, UsuarioId = 42, Nome = "Carro", Cor = "#2a78d6" };
        _categoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(categoria);

        await _sut.RemoverAsync(id: 1, usuarioId: 42);

        _categoriaRepository.Verify(r => r.Remove(categoria), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
