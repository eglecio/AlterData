﻿namespace Dominio.ModelosDTO {
  public class ClienteDTO {
    public int Id { get; set; }
    public string Nome { get; set; }

    public string CPF { get; set; }
    public string Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Observacao { get; set; }
    //public DateTime DataCadastro { get; set; } = DateTime.Today;
  }
}
