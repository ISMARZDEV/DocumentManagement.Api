using Bhd.Application.DTOs.UserDTOs;
using FluentValidation;

namespace Bhd.Application.Validators;

public class LoginUserValidator : AbstractValidator<UserLoginDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("Formato de email inválido")
            .Matches(@"^[a-zA-Z0-9._%+-]+@prueba\.com$").WithMessage("El correo debe tener el dominio @prueba.com");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria");
    }
}