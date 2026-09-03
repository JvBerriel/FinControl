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
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;
    private readonly IValidator<CriarCategoriaDto> _criarValidator;
    private readonly IValidator<AtualizarCategoriaDto> _atualizarValidator;

    public CategoriasController(
        ICategoriaService categoriaService,
        IValidator<CriarCategoriaDto> criarValidator,
        IValidator<AtualizarCategoriaDto> atualizarValidator)
    {
        _categoriaService = categoriaService;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> Listar()
    {
        return Ok(await _categoriaService.ListarPorUsuarioAsync(User.ObterUsuarioId()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> ObterPorId(int id)
    {
        return Ok(await _categoriaService.ObterPorIdAsync(id, User.ObterUsuarioId()));
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Criar(CriarCategoriaDto dto)
    {
        await _criarValidator.ValidateAndThrowAsync(dto);
        var categoria = await _categoriaService.CriarAsync(User.ObterUsuarioId(), dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> Atualizar(int id, AtualizarCategoriaDto dto)
    {
        await _atualizarValidator.ValidateAndThrowAsync(dto);
        return Ok(await _categoriaService.AtualizarAsync(id, User.ObterUsuarioId(), dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        await _categoriaService.RemoverAsync(id, User.ObterUsuarioId());
        return NoContent();
    }
}
