using FinControl.Application.DTOs;

namespace FinControl.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegistrarUsuarioDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
