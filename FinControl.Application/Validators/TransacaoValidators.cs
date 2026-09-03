using FinControl.Application.DTOs;
using FluentValidation;

namespace FinControl.Application.Validators;

public class CriarTransacaoDtoValidator : AbstractValidator<CriarTransacaoDto>
{
    public CriarTransacaoDtoValidator()
    {
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.DataTransacao).NotEmpty();
    }
}

public class AtualizarTransacaoDtoValidator : AbstractValidator<AtualizarTransacaoDto>
{
    public AtualizarTransacaoDtoValidator()
    {
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.DataTransacao).NotEmpty();
    }
}
