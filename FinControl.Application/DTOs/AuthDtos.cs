namespace FinControl.Application.DTOs;

public record RegistrarUsuarioDto(string Nome, string Email, string Senha, decimal RendaMensal);

public record LoginDto(string Email, string Senha);

public record AuthResponseDto(int UsuarioId, string Nome, string Email, string Token, DateTime ExpiraEm);
