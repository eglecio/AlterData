using Dominio.Entidades;
using Dominio.Validacao;
using FluentValidation.TestHelper;
using System.Xml.XPath;

namespace Testes.Dominio {

  public class ClienteValidadorTestes {

    private readonly ClienteValidador _validador;

    public ClienteValidadorTestes() {
      _validador = new ClienteValidador();
    }

    [Fact]
    public void Nome_DeveSerObrigatorio() {
      var cliente = new Cliente { Nome = "", CPF = "12345678901", Email = "teste@exemplo.com" };
      _validador.TestValidate(cliente).ShouldHaveValidationErrorFor(x => x.Nome);
    }


    [Theory(DisplayName = "Teste de CPF Validos")]
    [InlineData("123.456.789-09")]
    [InlineData("987.654.321-00")]
    [InlineData("741.852.963-55")]
    public void CPF_Valido_DevePassar(string cpf) {
      var cliente = new Cliente {
        Nome = "Teste",
        CPF = cpf,
        Email = "teste@exemplo.com"
      };
      var resultado = _validador.TestValidate(cliente);
      Assert.True(resultado.IsValid);
      resultado.ShouldNotHaveValidationErrorFor(x => x.CPF);
    }

    [Theory(DisplayName = "Teste de CPF Invalidos")]
    [InlineData("123.456.789-10")]
    [InlineData("987.654.321-12")]
    [InlineData("852.741.963-19")]
    [InlineData("456.123.789-89")]
    [InlineData("741.852.963-56")]
    [InlineData("009.261.539-27Das")]
    public void CPF_Invalido_DeveFalhar(string cpf) {
      var cliente = new Cliente {
        Nome = "Teste",
        CPF = cpf,
        Email = "teste@exemplo.com"
      };
      var resultado = _validador.TestValidate(cliente);
      Assert.False(resultado.IsValid);
      resultado.ShouldHaveValidationErrorFor(x => x.CPF);
    }

    [Theory(DisplayName = "Teste de email Invalido")]
    [InlineData("xxxxxx")]
    [InlineData("xxxxx@")]
    [InlineData("@xxxxxxx")]
    [InlineData("@xxxx.com")]
    [InlineData("xxxxx.com")]
    [InlineData("emailinvalido")]
    public void Email_Invalido_DeveFalhar(string email) {
      var cliente = new Cliente {
        Nome = "Teste",
        CPF = "52998224725",
        Email = email
      };

      var resultado = _validador.TestValidate(cliente);
      Assert.False(resultado.IsValid);
      resultado.ShouldHaveValidationErrorFor(x => x.Email);      
    }

    [Theory(DisplayName = "Teste de email Valido")]
    [InlineData("xxxxxx@yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy.com")]
    [InlineData("x@x.net")]
    [InlineData("x@xxxxxxx.xxxx")]
    [InlineData("email@valido.net")]
    public void Email_Valido_DevePassar(string email) {
      var cliente = new Cliente {
        Nome = "Teste",
        CPF = "52998224725",
        Email = email
      };

      var resultado = _validador.TestValidate(cliente);
      Assert.True(resultado.IsValid);
      resultado.ShouldNotHaveValidationErrorFor(x => x.Email);
    }


  }
}