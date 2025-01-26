using Dominio.Entidades;
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
          .Must(ValidarCPF).WithMessage("CPF inválido")
          .NotEmpty().WithMessage("CPF é obrigatório");

      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email é obrigatório")
          .EmailAddress().WithMessage("Email inválido");
    }

    private bool ValidarCPF(string cpf) {
      // Remover caracteres não numéricos
      cpf = new string(cpf.Where(char.IsDigit).ToArray());

      // Validar se tem 11 dígitos
      if (cpf.Length != 11)
        return false;

      // Validar dígitos verificadores
      int soma1 = 0, soma2 = 0;
      for (int i = 0; i < 9; i++) {
        soma1 += int.Parse(cpf[i].ToString()) * (10 - i);
        soma2 += int.Parse(cpf[i].ToString()) * (11 - i);
      }

      int digito1 = 11 - (soma1 % 11);
      digito1 = digito1 >= 10 ? 0 : digito1;

      soma2 += digito1 * 2;
      int digito2 = 11 - (soma2 % 11);
      digito2 = digito2 >= 10 ? 0 : digito2;

      return cpf.EndsWith(digito1.ToString() + digito2.ToString());
    }
  }
}
