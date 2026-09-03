using FinControl.Application.DTOs;
using FinControl.Application.Exceptions;
using FinControl.Application.Interfaces;
using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;

namespace FinControl.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoriaService(ICategoriaRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoriaDto>> ListarPorUsuarioAsync(int usuarioId)
    {
        var categorias = await _categoriaRepository.GetByUsuarioIdAsync(usuarioId);
        return categorias.Select(ParaDto);
    }

    public async Task<CategoriaDto> ObterPorIdAsync(int id, int usuarioId)
    {
        var categoria = await ObterCategoriaDoUsuarioAsync(id, usuarioId);
        return ParaDto(categoria);
    }

    public async Task<CategoriaDto> CriarAsync(int usuarioId, CriarCategoriaDto dto)
    {
        var categoria = new Categoria
        {
            UsuarioId = usuarioId,
            Nome = dto.Nome,
            Cor = dto.Cor,
            Icone = dto.Icone,
            Ativa = true
        };

        await _categoriaRepository.AddAsync(categoria);
        await _unitOfWork.SaveChangesAsync();

        return ParaDto(categoria);
    }

    public async Task<CategoriaDto> AtualizarAsync(int id, int usuarioId, AtualizarCategoriaDto dto)
    {
        var categoria = await ObterCategoriaDoUsuarioAsync(id, usuarioId);

        categoria.Nome = dto.Nome;
        categoria.Cor = dto.Cor;
        categoria.Icone = dto.Icone;
        categoria.Ativa = dto.Ativa;

        _categoriaRepository.Update(categoria);
        await _unitOfWork.SaveChangesAsync();

        return ParaDto(categoria);
    }

    public async Task RemoverAsync(int id, int usuarioId)
    {
        var categoria = await ObterCategoriaDoUsuarioAsync(id, usuarioId);

        _categoriaRepository.Remove(categoria);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Categoria> ObterCategoriaDoUsuarioAsync(int id, int usuarioId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id);
        if (categoria is null || categoria.UsuarioId != usuarioId)
            throw new NotFoundException($"Categoria {id} não encontrada.");

        return categoria;
    }

    private static CategoriaDto ParaDto(Categoria c) => new(c.Id, c.UsuarioId, c.Nome, c.Cor, c.Icone, c.Ativa);
}
