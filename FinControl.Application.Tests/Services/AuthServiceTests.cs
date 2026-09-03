using FinControl.Application.DTOs;
using FinControl.Application.Services;
using FinControl.Application.Settings;
using FinControl.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace FinControl.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<Usuario>> _userManager = CriarUserManagerMock();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var jwtSettings = Options.Create(new JwtSettings
        {
            Key = "chave-de-teste-com-tamanho-suficiente-para-hmac-sha256",
            Issuer = "FinControl.Api.Tests",
            Audience = "FinControl.Client.Tests",
            ExpirationMinutes = 60,
        });

        _sut = new AuthService(_userManager.Object, jwtSettings);
    }

    private static Mock<UserManager<Usuario>> CriarUserManagerMock()
    {
        var store = new Mock<IUserStore<Usuario>>();
        return new Mock<UserManager<Usuario>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task RegistrarAsync_QuandoCriacaoComSucesso_DeveRetornarTokenComDadosDoUsuario()
    {
        _userManager
            .Setup(m => m.CreateAsync(It.IsAny<Usuario>(), "SenhaForte123"))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<Usuario, string>((usuario, _) => usuario.Id = 42);

        var dto = new RegistrarUsuarioDto("Joao Vitor", "joao@example.com", "SenhaForte123", 6000m);

        var resultado = await _sut.RegistrarAsync(dto);

        Assert.Equal(42, resultado.UsuarioId);
        Assert.Equal("Joao Vitor", resultado.Nome);
        Assert.Equal("joao@example.com", resultado.Email);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
        Assert.True(resultado.ExpiraEm > DateTime.UtcNow);
    }

    [Fact]
    public async Task RegistrarAsync_QuandoFalhaCriacao_DeveLancarValidationExceptionComErrosDoIdentity()
    {
        var erro = new IdentityError { Code = "DuplicateEmail", Description = "O e-mail já está cadastrado." };
        _userManager
            .Setup(m => m.CreateAsync(It.IsAny<Usuario>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(erro));

        var dto = new RegistrarUsuarioDto("Joao Vitor", "joao@example.com", "SenhaForte123", 6000m);

        var excecao = await Assert.ThrowsAsync<ValidationException>(() => _sut.RegistrarAsync(dto));
        Assert.Contains(excecao.Errors, e => e.ErrorMessage == "O e-mail já está cadastrado.");
    }

    [Fact]
    public async Task LoginAsync_QuandoUsuarioNaoExiste_DeveLancarUnauthorizedAccessException()
    {
        _userManager.Setup(m => m.FindByEmailAsync("naoexiste@example.com")).ReturnsAsync((Usuario?)null);

        var dto = new LoginDto("naoexiste@example.com", "qualquer");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_QuandoSenhaIncorreta_DeveLancarUnauthorizedAccessException()
    {
        var usuario = new Usuario { Id = 1, Email = "joao@example.com", Nome = "Joao" };
        _userManager.Setup(m => m.FindByEmailAsync("joao@example.com")).ReturnsAsync(usuario);
        _userManager.Setup(m => m.CheckPasswordAsync(usuario, "senhaErrada")).ReturnsAsync(false);

        var dto = new LoginDto("joao@example.com", "senhaErrada");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_QuandoCredenciaisValidas_DeveRetornarToken()
    {
        var usuario = new Usuario { Id = 7, Email = "joao@example.com", Nome = "Joao" };
        _userManager.Setup(m => m.FindByEmailAsync("joao@example.com")).ReturnsAsync(usuario);
        _userManager.Setup(m => m.CheckPasswordAsync(usuario, "SenhaCorreta1")).ReturnsAsync(true);

        var dto = new LoginDto("joao@example.com", "SenhaCorreta1");

        var resultado = await _sut.LoginAsync(dto);

        Assert.Equal(7, resultado.UsuarioId);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
    }
}
