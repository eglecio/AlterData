using API.Models;
using FluentValidation;

namespace API.ValidacaoDTO {

  public class UsuarioValidador : AbstractValidator<UsuarioLoginDTO> {

    public UsuarioValidador() {
      RuleFor(x => x.Login)
          .NotEmpty().WithMessage("Login é obrigatório")
          .NotNull().WithMessage("Login não pode ser nulo")
          .MinimumLength(2).WithMessage("Nome deve ter no mínimo 3 caracteres");

      RuleFor(x => x.Senha)
          .NotEmpty().WithMessage("Senha é obrigatória")
          .NotNull().WithMessage("Senha não pode ser nula");
    }

  }

}
