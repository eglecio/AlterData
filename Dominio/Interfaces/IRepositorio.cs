using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dominio.Interfaces {

  public interface IRepositorio<T> : IDisposable where T : class {
    Task<T> AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task RemoverAsync(T entidade);
    Task RemoverPorIdAsync(object id);

    // Métodos de Busca...
    Task<T> ObterPorIdAsync(object id);
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro);
    Task<IEnumerable<T>> BuscarPaginadoAsync(
        Expression<Func<T, bool>> filtro,
        int pagina,
        int itensPorPagina,
        Expression<Func<T, object>>? ordenacao = null
    );

  }
}
