using Dominio.Entidades;
using Dominio.ModelosDTO;
using Dominio.Validacao;
using FluentValidation.TestHelper;
using System.Xml.XPath;

namespace Testes.Dominio {

  public class UsuarioLoginDTOValidadorTestes {

    private readonly UsuarioLoginDTOValidador _validador;

    public UsuarioLoginDTOValidadorTestes() {
      _validador = new UsuarioLoginDTOValidador();
    }

    [Fact]
    public void Senha_DeveSerObrigatoria() {
      var cliente = new UsuarioLoginDTO { Login = "teste@teste.com", Senha = string.Empty };
      _validador.TestValidate(cliente).ShouldHaveValidationErrorFor(x => x.Senha);
    }


    [Theory(DisplayName = "Teste de senha Invalidos")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("   a")]
    [InlineData("  a ")]
    [InlineData(" a  ")]
    [InlineData("a   ")]
    public void Senha_DeveTerTamanhoMinimoObrigatoria(string senha) {
      var cliente = new UsuarioLoginDTO { Login = "teste@teste.com", Senha = senha };
      _validador.TestValidate(cliente).ShouldHaveValidationErrorFor(x => x.Senha);
    }


    [Theory(DisplayName = "Teste de login Invalido")]
    [InlineData("xxxxxx")]
    [InlineData("xxxxx@")]
    [InlineData("@xxxxxxx")]
    [InlineData("@xxxx.com")]
    [InlineData("xxxxx.com")]
    [InlineData("emailinvalido")]
    public void Login_Invalido_DeveFalhar(string login) {
      var usuarioDTO = new UsuarioLoginDTO { Senha = "123456", Login = login };
      var resultado = _validador.TestValidate(usuarioDTO);
      Assert.False(resultado.IsValid);
      resultado.ShouldHaveValidationErrorFor(x => x.Login);
    }

    [Theory(DisplayName = "Teste de login Valido")]
    [InlineData("xxxxxx@yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy.com")]
    [InlineData("x@x.net")]
    [InlineData("x@xxxxxxx.xxxx")]
    [InlineData("email@valido.net")]
    public void Login_Valido_DevePassar(string login) {
      var usuarioDTO = new UsuarioLoginDTO { Senha = "123456", Login = login };

      var resultado = _validador.TestValidate(usuarioDTO);
      //Assert.True(resultado.IsValid);
      resultado.ShouldNotHaveValidationErrorFor(x => x.Login);
    }


  }
}