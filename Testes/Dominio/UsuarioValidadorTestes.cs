using Dominio.Entidades;
using Dominio.Validacao;
using FluentValidation.TestHelper;


namespace Testes.Dominio {

  public class UsuarioValidadorTestes {

    private readonly UsuarioValidador _validador;

    public UsuarioValidadorTestes() {
      _validador = new UsuarioValidador();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Email_QuandoVazioOuNulo_DeveRetornarErro(string email) {
      var usuario = new Usuario { Email = email, Senha = "senha123" };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("email@")]
    [InlineData("@dominio.com")]
    [InlineData("email.com")]
    public void Email_QuandoFormatoInvalido_DeveRetornarErro(string email) {
      var usuario = new Usuario { Email = email, Senha = "senha123" };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email precisa conter @");
    }

    [Theory]
    [InlineData("usuario@dominio.com")]
    [InlineData("teste@teste.com.br")]
    [InlineData("usuario.nome@empresa.net")]
    public void Email_QuandoFormatoValido_NaoDeveRetornarErro(string email) {
      var usuario = new Usuario { Email = email, Senha = "senha123" };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Senha_QuandoVaziaOuNula_DeveRetornarErro(string senha) {
      var usuario = new Usuario { Email = "teste@teste.com", Senha = senha };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("ab")]
    [InlineData("x")]
    public void Senha_QuandoMenorQue4Caracteres_DeveRetornarErro(string senha) {
      var usuario = new Usuario { Email = "teste@teste.com", Senha = senha };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldHaveValidationErrorFor(x => x.Senha).WithErrorMessage("Senha deve ter no mínimo 4 caracteres");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("senha123")]
    [InlineData("AbCd1234")]
    [InlineData("!@#$%^&*")]
    public void Senha_QuandoValida_NaoDeveRetornarErro(string senha) {
      var usuario = new Usuario { Email = "teste@teste.com", Senha = senha };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldNotHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Validacao_QuandoUsuarioValido_NaoDeveRetornarErros() {
      var usuario = new Usuario { Email = "teste@teste.com", Senha = "senha123" };

      var resultado = _validador.TestValidate(usuario);
      resultado.ShouldNotHaveAnyValidationErrors();
    }
  }

}