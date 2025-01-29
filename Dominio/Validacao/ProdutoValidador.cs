using Dominio.Entidades;

namespace Dominio.Validacao {

  using FluentValidation;

  public class ProdutoValidator : AbstractValidator<Produto> {

    public ProdutoValidator() {
      RuleFor(produto => produto.Nome)
          .NotEmpty().WithMessage("O nome do produto é obrigatório")
          .Length(3, 100)
          .WithMessage("O nome do produto deve ter entre 3 e 100 caracteres");

      RuleFor(produto => produto.Observacao)
          .MaximumLength(2048)
          .WithMessage("A observação não pode ter mais que 2048 caracteres")
          .When(produto => !string.IsNullOrEmpty(produto.Observacao));

      RuleFor(produto => produto.QuantidadeEstoque)
          .GreaterThanOrEqualTo(0)
          .WithMessage("A quantidade em estoque não pode ser negativa");

      RuleFor(produto => produto.ValorCusto)
          .GreaterThan(0)
          .WithMessage("O valor de custo deve ser maior que zero")
          .LessThanOrEqualTo(produto => produto.ValorVenda)
          .WithMessage("O valor de custo não pode ser maior que o valor de venda");

      RuleFor(produto => produto.ValorVenda)
          .GreaterThan(0)
          .WithMessage("O valor de venda deve ser maior que zero");

      RuleFor(produto => produto.DataCadastro)
          .NotEmpty()
          .WithMessage("A data de cadastro é obrigatória")
          .LessThanOrEqualTo(DateTime.Today)
          .WithMessage("A data de cadastro não pode ser futura");
    }
  }
}
