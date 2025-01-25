using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicos {

  public abstract class RepositorioBase<T, TContext>(TContext context) : IRepositorio<T>
    where T : class
    where TContext : DbContext {

    protected readonly TContext _contextoBancoDados = context ?? throw new ArgumentNullException(nameof(context));
    protected readonly DbSet<T> _dbSet = context.Set<T>();
    private bool _disposed = false;

    #region #### Metodos privados ####

    private static void LogarErro(Exception ex) {
      var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "repositorio_erros.log");

      try {
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)); // Garantia que o diretório de logs exista...

        var logMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] " +
            $"Erro: {ex.Message}\n" +
            $"Tipo: {ex.GetType().FullName}\n" +
            $"Stacktrace: {ex.StackTrace}\n" +
            "-------------------------------------------\n";

        File.AppendAllText(logPath, logMessage);
      }
      catch {
        Console.Error.WriteLine($"Erro ao registrar log: {ex.Message}");
      }
    }
    #endregion


    #region #### CRUD Basico  ####

    public async Task<T> AdicionarAsync(T entidade) {
      if (entidade == null) throw new ArgumentNullException(nameof(entidade), "Entidade não pode ser nula.");

      await _dbSet.AddAsync(entidade);
      await _contextoBancoDados.SaveChangesAsync();
      return entidade;
    }

    public async Task AtualizarAsync(T entidade) {
      if (entidade == null) throw new ArgumentNullException(nameof(entidade), "Entidade não pode ser nula.");

      _contextoBancoDados.Entry(entidade).State = EntityState.Modified;
      await _contextoBancoDados.SaveChangesAsync();
    }

    //public async Task RemoverAsync(T entidade) {
    //  if (entidade == null)
    //    throw new ArgumentNullException(nameof(entidade), "Entidade não pode ser nula.");

    //  try {
    //    var entry = _contextoBancoDados.Entry(entidade);

    //    if (entry.State == EntityState.Detached) { // Caso não está sendo rastreada anexamos ela...
    //      _dbSet.Attach(entidade);
    //    }

    //    _dbSet.Remove(entidade);
    //    await _contextoBancoDados.SaveChangesAsync();
    //  }
    //  catch (DbUpdateException ex) {
    //    LogarErro(ex);
    //    throw new RepositorioException("Erro ao remover entidade.", ex);
    //  }
    //  catch (Exception ex) {
    //    LogarErro(ex);
    //    throw;
    //  }
    //}

    public async Task RemoverAsync(T entidade) {
      if (entidade == null)
        throw new ArgumentNullException(nameof(entidade), "Entidade não pode ser nula.");

      try {
        (entidade as EntidadeBase).Excluido = true;
        _contextoBancoDados.Entry(entidade).State = EntityState.Modified;
        await _contextoBancoDados.SaveChangesAsync();
      }
      catch (DbUpdateException ex) {
        LogarErro(ex);
        throw new RepositorioException("Erro ao remover entidade por elemento.", ex);
      }
      catch (Exception ex) {
        LogarErro(ex);
        throw;
      }
    }

    //public async Task RemoverPorIdAsync(object id) {
    //  if (id == null)
    //    throw new ArgumentNullException(nameof(id), "Id não pode ser nulo ou inválido.");

    //  try {
    //    var entidade = await _dbSet.FindAsync(id) ??
    //      throw new RepositorioException($"Entidade com ID {id} não encontrada.", RepositorioException.ErrorCode.ErroNaoEncontrado);

    //    _dbSet.Remove(entidade);
    //    await _contextoBancoDados.SaveChangesAsync();
    //  }
    //  catch (DbUpdateException ex) {
    //    LogarErro(ex);
    //    throw new RepositorioException("Erro ao remover entidade por ID.", ex);
    //  }
    //  catch (Exception ex) {
    //    LogarErro(ex);
    //    throw;
    //  }
    //}

    public async Task RemoverPorIdAsync(object id) {
      if (id == null)
        throw new ArgumentNullException(nameof(id), "Id não pode ser nulo ou inválido.");

      try {
        var entidade = await _dbSet.FindAsync(id) ??
          throw new RepositorioException($"Entidade com ID {id} não encontrada.", RepositorioException.ErrorCode.ErroNaoEncontrado);

        await RemoverAsync(entidade);
      }
      catch (DbUpdateException ex) {
        LogarErro(ex);
        throw new RepositorioException("Erro ao remover entidade por ID.", ex);
      }
      catch (Exception ex) {
        LogarErro(ex);
        throw;
      }
    }


    #endregion


    #region #### Metodos de Pesquisa ####
    public async Task<T> ObterPorIdAsync(object id) {
      if (id == null)
        throw new ArgumentNullException(nameof(id), "Identificador não pode ser nulo.");

      try {
        return
          await _dbSet.FindAsync(id) ??
          throw new RepositorioException($"Entidade com ID {id} não encontrada.", RepositorioException.ErrorCode.ErroNaoEncontrado);
      }
      catch (Exception ex) when (ex is not RepositorioException) {
        LogarErro(ex);
        throw new RepositorioException("Erro ao obter entidade por ID.", ex);
      }
    }

    public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro) {
      if (filtro == null)
        throw new ArgumentNullException(nameof(filtro), "Filtro não pode ser nulo.");

      try {
        return await _dbSet.AsNoTracking().Where(filtro).ToListAsync();
      }
      catch (Exception ex) {
        LogarErro(ex);
        throw new RepositorioException("Erro ao buscar entidades.", ex);
      }
    }

    public async Task<IEnumerable<T>> BuscarPaginadoAsync(Expression<Func<T, bool>> filtro, int pagina, int itensPorPagina, Expression<Func<T, object>>? ordenacao = null) {
      if (pagina < 1)
        throw new ArgumentException("Número da página deve ser maior que zero.", nameof(pagina));

      if (itensPorPagina < 1)
        throw new ArgumentException("Itens por página deve ser maior que zero.", nameof(itensPorPagina));

      try {
        IQueryable<T> consulta = _dbSet.AsNoTracking();

        if (filtro != null)
          consulta = consulta.Where(filtro);

        if (ordenacao != null)
          consulta = consulta.OrderBy(ordenacao);
        else
          consulta = consulta.OrderBy(e => e.GetType().GetProperty("Id"));

        return await consulta
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync();
      }
      catch (Exception ex) {
        LogarErro(ex);
        throw new RepositorioException("Erro ao buscar entidades paginadas.", ex);
      }
    }

    #endregion


    // Gerenciamento e otmização de memoria (usado pelo proprio .net e plo GC)...
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (!_disposed) {
        if (disposing) {
          _contextoBancoDados?.Dispose();
        }
        _disposed = true;
      }
    }

  }
}
