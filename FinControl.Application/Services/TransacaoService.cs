using FinControl.Application.DTOs;
using FinControl.Application.Exceptions;
using FinControl.Application.Interfaces;
using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;

namespace FinControl.Application.Services;

public class TransacaoService : ITransacaoService
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransacaoService(
        ITransacaoRepository transacaoRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _transacaoRepository = transacaoRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TransacaoDto>> ListarPorUsuarioAsync(int usuarioId)
    {
        var transacoes = await _transacaoRepository.GetByUsuarioIdAsync(usuarioId);
        return transacoes.Select(ParaDto);
    }

    public async Task<TransacaoDto> ObterPorIdAsync(int id, int usuarioId)
    {
        var transacao = await ObterTransacaoDoUsuarioAsync(id, usuarioId);
        return ParaDto(transacao);
    }

    public async Task<TransacaoDto> CriarAsync(int usuarioId, CriarTransacaoDto dto)
    {
        await GarantirCategoriaDoUsuarioAsync(dto.CategoriaId, usuarioId);

        var transacao = new Transacao
        {
            UsuarioId = usuarioId,
            CategoriaId = dto.CategoriaId,
            Descricao = dto.Descricao,
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            DataTransacao = dto.DataTransacao
        };

        await _transacaoRepository.AddAsync(transacao);
        await _unitOfWork.SaveChangesAsync();

        var criada = await _transacaoRepository.GetByIdComCategoriaAsync(transacao.Id)
            ?? throw new NotFoundException($"Transação {transacao.Id} não encontrada.");
        return ParaDto(criada);
    }

    public async Task<TransacaoDto> AtualizarAsync(int id, int usuarioId, AtualizarTransacaoDto dto)
    {
        await ObterTransacaoDoUsuarioAsync(id, usuarioId);
        await GarantirCategoriaDoUsuarioAsync(dto.CategoriaId, usuarioId);

        var transacao = await _transacaoRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Transação {id} não encontrada.");

        transacao.CategoriaId = dto.CategoriaId;
        transacao.Descricao = dto.Descricao;
        transacao.Valor = dto.Valor;
        transacao.Tipo = dto.Tipo;
        transacao.DataTransacao = dto.DataTransacao;

        _transacaoRepository.Update(transacao);
        await _unitOfWork.SaveChangesAsync();

        var atualizada = await _transacaoRepository.GetByIdComCategoriaAsync(id)
            ?? throw new NotFoundException($"Transação {id} não encontrada.");
        return ParaDto(atualizada);
    }

    public async Task RemoverAsync(int id, int usuarioId)
    {
        var transacao = await ObterTransacaoDoUsuarioAsync(id, usuarioId);

        _transacaoRepository.Remove(transacao);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Transacao> ObterTransacaoDoUsuarioAsync(int id, int usuarioId)
    {
        var transacao = await _transacaoRepository.GetByIdComCategoriaAsync(id);
        if (transacao is null || transacao.UsuarioId != usuarioId)
            throw new NotFoundException($"Transação {id} não encontrada.");

        return transacao;
    }

    private async Task GarantirCategoriaDoUsuarioAsync(int categoriaId, int usuarioId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria is null || categoria.UsuarioId != usuarioId)
            throw new NotFoundException($"Categoria {categoriaId} não encontrada.");
    }

    private static TransacaoDto ParaDto(Transacao t) => new(
        t.Id,
        t.UsuarioId,
        t.CategoriaId,
        t.Categoria?.Nome ?? string.Empty,
        t.Descricao,
        t.Valor,
        t.Tipo,
        t.DataTransacao);
}
