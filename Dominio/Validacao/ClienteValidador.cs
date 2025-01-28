using Dominio.Entidades;
using Dominio.Helper;
using FluentValidation;

namespace Dominio.Validacao {

  public class ClienteValidador : AbstractValidator<Cliente> {

    public ClienteValidador() {
      RuleFor(x => x.Nome)
          .NotEmpty().WithMessage("Nome é obrigatório")
          .NotNull().WithMessage("Nome não pode ser nulo")
          .MinimumLength(2).WithMessage("Nome deve ter no mínimo 2 caracteres")
          .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres");

      RuleFor(x => x.CPF)
          .NotEmpty().WithMessage("CPF é obrigatório")
          .Must(cpf => cpf == null || cpf.All(c => char.IsDigit(c) || c == '.' || c == '-'))
              .WithMessage("CPF deve conter apenas números, pontos e hífen")
          .Must(cpf => cpf == null || HelperCpf.Validar(cpf))
              .WithMessage("CPF inválido");

      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email é obrigatório")
          .EmailAddress().WithMessage("Email inválido");
    }
  }
}
