using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Helper {
  public static class HelperCpf {
    public static string RemoverFormatacao(string cpf) {
      if (string.IsNullOrEmpty(cpf)) return "";
      return new string(cpf.Where(char.IsDigit).ToArray());
    }

    public static string Formatar(string cpf) {
      cpf = RemoverFormatacao(cpf);
      if (cpf.Length != 11) return cpf;
      return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
    }

    public static bool Validar(string cpf) {
      // Remove formatação
      cpf = RemoverFormatacao(cpf);

      // Verifica se tem 11 dígitos
      if (cpf.Length != 11) return false;

      // Verifica se todos são números
      if (!cpf.All(char.IsDigit)) return false;

      // Verifica se são números repetidos
      if (cpf.Distinct().Count() == 1) return false;

      int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
      int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

      string tempCpf = cpf.Substring(0, 9);
      int soma = 0;

      for (int i = 0; i < 9; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

      int resto = soma % 11;
      resto = resto < 2 ? 0 : 11 - resto;

      string digito = resto.ToString();
      tempCpf = tempCpf + digito;
      soma = 0;

      for (int i = 0; i < 10; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

      resto = soma % 11;
      resto = resto < 2 ? 0 : 11 - resto;

      digito = digito + resto.ToString();

      return cpf.EndsWith(digito);
    }
  }
}
