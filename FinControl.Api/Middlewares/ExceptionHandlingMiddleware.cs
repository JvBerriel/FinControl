using System.Net;
using System.Text.Json;
using FinControl.Application.Exceptions;
using FluentValidation;

namespace FinControl.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.BadRequest, new
            {
                mensagem = "Um ou mais erros de validação ocorreram.",
                erros = ex.Errors.Select(e => new { campo = e.PropertyName, erro = e.ErrorMessage })
            });
        }
        catch (NotFoundException ex)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.NotFound, new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.Unauthorized, new { mensagem = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await EscreverRespostaAsync(context, HttpStatusCode.BadRequest, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar a requisição {Path}", context.Request.Path);
            await EscreverRespostaAsync(context, HttpStatusCode.InternalServerError,
                new { mensagem = "Ocorreu um erro interno ao processar a requisição." });
        }
    }

    private static Task EscreverRespostaAsync(HttpContext context, HttpStatusCode statusCode, object corpo)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(corpo));
    }
}
