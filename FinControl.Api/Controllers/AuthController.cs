using FinControl.Application.DTOs;
using FinControl.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FinControl.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegistrarUsuarioDto> registrarValidator,
        IValidator<LoginDto> loginValidator)
    {
        _authService = authService;
        _registrarValidator = registrarValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<AuthResponseDto>> Registrar(RegistrarUsuarioDto dto)
    {
        await _registrarValidator.ValidateAndThrowAsync(dto);
        return Ok(await _authService.RegistrarAsync(dto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        await _loginValidator.ValidateAndThrowAsync(dto);
        return Ok(await _authService.LoginAsync(dto));
    }
}
