using Dominio.Entidades;
using FluentValidation;

namespace Dominio.Validacao {
  public class UsuarioValidador : AbstractValidator<Usuario> {

    public UsuarioValidador() {

      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email é obrigatório")
          .NotNull().WithMessage("Email não pode ser nulo")
          .EmailAddress().WithMessage("Email precisa conter @");

      RuleFor(x => x.Senha)
        .NotEmpty().WithMessage("Senha é obrigatória")
        .NotNull().WithMessage("Senha não pode ser nula")
        .MinimumLength(4).WithMessage("Senha deve ter no mínimo 4 caracteres")
        .Must(senha => !string.IsNullOrWhiteSpace(senha) &&
                        senha.Replace(" ", "").Length >= 4)
              .WithMessage("Senha deve ter no mínimo 4 caracteres sem espaços em branco")
        .Must(senha => !senha.All(char.IsWhiteSpace)).WithMessage("Senha não pode conter espaços em branco");

    }
  }
}
