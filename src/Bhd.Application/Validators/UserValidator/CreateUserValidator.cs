using Bhd.Application.DTOs.UserDTOs;
using FluentValidation;

namespace Bhd.Application.Validators;

public class CreateUserValidator : AbstractValidator<UserCreateDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("Formato de email inválido")
            .Matches(@"^[a-zA-Z0-9._%+-]+@prueba\.com$").WithMessage("El correo debe tener el dominio @prueba.com");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe tener al menos una letra mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe tener al menos una letra minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe tener al menos un número")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("La contraseña debe tener al menos un carácter especial");
    }
}