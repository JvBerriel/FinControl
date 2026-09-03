using FinControl.Domain.Enums;

namespace FinControl.Application.DTOs;

public record TransacaoDto(
    int Id,
    int UsuarioId,
    int CategoriaId,
    string CategoriaNome,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    DateOnly DataTransacao);

public record CriarTransacaoDto(
    int CategoriaId,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    DateOnly DataTransacao);

public record AtualizarTransacaoDto(
    int CategoriaId,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    DateOnly DataTransacao);
