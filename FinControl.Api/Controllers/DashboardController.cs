using FinControl.Api.Extensions;
using FinControl.Application.DTOs;
using FinControl.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinControl.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("resumo-mensal")]
    public async Task<ActionResult<ResumoMensalDto>> ResumoMensal([FromQuery] int mes, [FromQuery] int ano)
    {
        return Ok(await _dashboardService.ObterResumoMensalAsync(User.ObterUsuarioId(), mes, ano));
    }

    [HttpGet("medias-por-categoria")]
    public async Task<ActionResult<IEnumerable<MediaCategoriaDto>>> MediasPorCategoria([FromQuery] int quantidadeMeses = 6)
    {
        return Ok(await _dashboardService.ObterMediasPorCategoriaAsync(User.ObterUsuarioId(), quantidadeMeses));
    }

    [HttpGet("sugestao-investimento")]
    public async Task<ActionResult<SugestaoInvestimentoDto>> SugestaoInvestimento(
        [FromQuery] int mes, [FromQuery] int ano, [FromQuery] decimal percentualReserva = 0.20m)
    {
        return Ok(await _dashboardService.ObterSugestaoInvestimentoAsync(User.ObterUsuarioId(), mes, ano, percentualReserva));
    }
}
