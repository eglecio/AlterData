using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicos {
  public class RepositorioException : Exception {
    public ErrorCode CodigoErro { get; }

    public RepositorioException(string mensagem) : base(mensagem) {
      CodigoErro = ErrorCode.ErroPadrao;
    }

    public RepositorioException(string mensagem, Exception innerException) : base(mensagem, innerException) {
      CodigoErro = ErrorCode.ErroPadrao;
    }

    public RepositorioException(string mensagem, ErrorCode codigoErro) : base(mensagem) {
      CodigoErro = codigoErro;
    }

    public RepositorioException(string mensagem, ErrorCode codigoErro, Exception innerException) : base(mensagem, innerException) {
      CodigoErro = codigoErro;
    }

    public enum ErrorCode {
      ErroPadrao = 0,
      ErroValidacao = 1,
      ErroConcorrencia = 2,
      ErroConexao = 3,
      ErroNaoEncontrado = 4
    }
  }
}
