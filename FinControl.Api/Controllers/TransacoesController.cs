using FinControl.Api.Extensions;
using FinControl.Application.DTOs;
using FinControl.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinControl.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _transacaoService;
    private readonly IValidator<CriarTransacaoDto> _criarValidator;
    private readonly IValidator<AtualizarTransacaoDto> _atualizarValidator;

    public TransacoesController(
        ITransacaoService transacaoService,
        IValidator<CriarTransacaoDto> criarValidator,
        IValidator<AtualizarTransacaoDto> atualizarValidator)
    {
        _transacaoService = transacaoService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoDto>>> Listar()
    {
        return Ok(await _transacaoService.ListarPorUsuarioAsync(User.ObterUsuarioId()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransacaoDto>> ObterPorId(int id)
    {
        return Ok(await _transacaoService.ObterPorIdAsync(id, User.ObterUsuarioId()));
    }

    [HttpPost]
    public async Task<ActionResult<TransacaoDto>> Criar(CriarTransacaoDto dto)
    {
        await _criarValidator.ValidateAndThrowAsync(dto);
        var transacao = await _transacaoService.CriarAsync(User.ObterUsuarioId(), dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = transacao.Id }, transacao);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransacaoDto>> Atualizar(int id, AtualizarTransacaoDto dto)
    {
        await _atualizarValidator.ValidateAndThrowAsync(dto);
        return Ok(await _transacaoService.AtualizarAsync(id, User.ObterUsuarioId(), dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _transacaoService.RemoverAsync(id, User.ObterUsuarioId());
        return NoContent();
    }
}
