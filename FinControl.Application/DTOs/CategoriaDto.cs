namespace FinControl.Application.DTOs;

public record CategoriaDto(int Id, int UsuarioId, string Nome, string Cor, string? Icone, bool Ativa);

public record CriarCategoriaDto(string Nome, string Cor, string? Icone);

public record AtualizarCategoriaDto(string Nome, string Cor, string? Icone, bool Ativa);
