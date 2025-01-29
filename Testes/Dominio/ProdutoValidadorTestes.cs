using Dominio.Entidades;
using Dominio.Validacao;
using FluentValidation.TestHelper;

namespace Testes.Dominio {

  public class ProdutoValidadorTestes {

    private readonly ProdutoValidator _validator;

    public ProdutoValidadorTestes() {
      _validator = new ProdutoValidator();
    }

    public static IEnumerable<object[]> ProdutosValidos() {
      yield return new object[]
      {
            new Produto
            {
                Nome = "Produto Teste",
                Observacao = "Observação teste",
                QuantidadeEstoque = 10,
                ValorCusto = 50,
                ValorVenda = 100,
                DataCadastro = DateTime.Today
            }
      };

      yield return new object[]
      {
            new Produto
            {
                Nome = "ABC",
                QuantidadeEstoque = 0,
                ValorCusto = 10,
                ValorVenda = 10,
                DataCadastro = DateTime.Today
            }
      };
    }

    [Theory]
    [MemberData(nameof(ProdutosValidos))]
    public void ValidarProduto_QuandoDadosValidos_DevePassarNaValidacao(Produto produto) {
      var resultado = _validator.TestValidate(produto);
      resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("ab")]
    [InlineData("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua plus")]
    public void ValidarNome_QuandoInvalido_DeveFalhar(string nomeInvalido) {
      var produto = new Produto { Nome = nomeInvalido };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.Nome);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ValidarQuantidadeEstoque_QuandoNegativa_DeveFalhar(double quantidadeInvalida) {
      var produto = new Produto { QuantidadeEstoque = quantidadeInvalida };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.QuantidadeEstoque);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarValorCusto_QuandoMenorOuIgualZero_DeveFalhar(double valorCustoInvalido) {
      var produto = new Produto { ValorCusto = valorCustoInvalido };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.ValorCusto);
    }

    [Fact]
    public void ValidarValorCusto_QuandoMaiorQueValorVenda_DeveFalhar() {
      var produto = new Produto { ValorCusto = 100, ValorVenda = 50 };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.ValorCusto);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarValorVenda_QuandoMenorOuIgualZero_DeveFalhar(double valorVendaInvalido) {
      var produto = new Produto { ValorVenda = valorVendaInvalido };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.ValorVenda);
    }

    [Fact]
    public void ValidarDataCadastro_QuandoDataFutura_DeveFalhar() {
      var produto = new Produto { DataCadastro = DateTime.Today.AddDays(1) };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.DataCadastro);
    }

    [Theory]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo plus more text to exceed 500 characters Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo plus more text to exceed 500 characters Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo plus more text to exceed 500 characters Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo plus more text to exceed 500 characters Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo plus more text to exceed 500 characters")]
    public void ValidarObservacao_QuandoMaiorQue2048Caracteres_DeveFalhar(string observacaoInvalida) {
      var produto = new Produto { Observacao = observacaoInvalida };

      var resultado = _validator.TestValidate(produto);
      resultado.ShouldHaveValidationErrorFor(p => p.Observacao);
    }
  }
}
