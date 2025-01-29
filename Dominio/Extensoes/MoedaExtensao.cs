using System.Globalization;

namespace Dominio.Extensoes {
  public static class MoedaExtensao {

    public static string ParaRealComSimbolo(this double value) {
      return string.Format(new CultureInfo("pt-BR"), "{0:C}", value);
    }

    public static string ParaRealSemSimbolo(this double value) {
      return string.Format(new CultureInfo("pt-BR"), "{0:N2}", value);
    }

    public static string ParaRealComSimbolo(this double value, int decimals) {
      var format = "{0:C" + decimals + "}";
      return string.Format(new CultureInfo("pt-BR"), format, value);
    }
  }
}
