using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades {
  public abstract class EntidadeBase {
    [Key]
    public int Id { get; set; }
    public bool Excluido { get; set; } = false;

  }
}
