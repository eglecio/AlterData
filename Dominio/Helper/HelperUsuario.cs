using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Helper {
  public static class HelperUsuario {
    public static string DescricaoPerfil(PerfilUsuario perfil) {
      if (perfil == PerfilUsuario.Admin) {
        return "Admin";
      }
      return perfil == PerfilUsuario.Padrao ? "Visualização" : "Edição";
    }
  }
}
