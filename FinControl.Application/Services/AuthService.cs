using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinControl.Application.DTOs;
using FinControl.Application.Interfaces;
using FinControl.Application.Settings;
using FinControl.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinControl.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserManager<Usuario> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        var usuario = new Usuario
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nome = dto.Nome,
            RendaMensal = dto.RendaMensal
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Senha);

        if (!resultado.Succeeded)
        {
            var falhas = resultado.Errors
                .Select(e => new FluentValidation.Results.ValidationFailure(e.Code, e.Description))
                .ToList();
            throw new ValidationException(falhas);
        }

        return GerarResposta(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _userManager.FindByEmailAsync(dto.Email);
        if (usuario is null || !await _userManager.CheckPasswordAsync(usuario, dto.Senha))
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        return GerarResposta(usuario);
    }

    private AuthResponseDto GerarResposta(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
            new(ClaimTypes.Name, usuario.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        var tokenSerializado = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto(usuario.Id, usuario.Nome, usuario.Email ?? string.Empty, tokenSerializado, expiraEm);
    }
}
