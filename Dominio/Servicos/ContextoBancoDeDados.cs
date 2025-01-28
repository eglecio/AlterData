using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Servicos {

  public class ContextoBancoDeDados : DbContext {
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    public ContextoBancoDeDados(DbContextOptions<ContextoBancoDeDados> options) : base(options) {
      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Usuario>()
          .HasIndex(c => c.Email)
          .IsUnique();

      modelBuilder.Entity<Cliente>()
          .HasIndex(c => c.CPF)
          .IsUnique();

      modelBuilder.Entity<Produto>();
    }
  }
}
