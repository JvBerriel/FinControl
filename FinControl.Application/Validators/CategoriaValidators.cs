using FinControl.Application.DTOs;
using FluentValidation;

namespace FinControl.Application.Validators;

public class CriarCategoriaDtoValidator : AbstractValidator<CriarCategoriaDto>
{
    public CriarCategoriaDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Cor).NotEmpty().Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Cor deve estar no formato hexadecimal, ex: #FF5733.");
        RuleFor(x => x.Icone).MaximumLength(50);
    }
}

public class AtualizarCategoriaDtoValidator : AbstractValidator<AtualizarCategoriaDto>
{
    public AtualizarCategoriaDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Cor).NotEmpty().Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Cor deve estar no formato hexadecimal, ex: #FF5733.");
        RuleFor(x => x.Icone).MaximumLength(50);
    }
}
