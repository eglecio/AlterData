using Dominio.ModelosDTO;
using FluentValidation;

namespace Dominio.Validacao {
  public class UsuarioLoginDTOValidador : AbstractValidator<UsuarioLoginDTO> {

    public UsuarioLoginDTOValidador() {

      RuleFor(x => x.Login)
          .NotEmpty().WithMessage("Login é obrigatório")
          .NotNull().WithMessage("Login não pode ser nulo")
          .EmailAddress().WithMessage("Seu login é o seu e-mail");

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
