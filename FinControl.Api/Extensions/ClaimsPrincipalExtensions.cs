using System.Security.Claims;

namespace FinControl.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int ObterUsuarioId(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token não contém identificação do usuário.");
        return int.Parse(valor);
    }
}
