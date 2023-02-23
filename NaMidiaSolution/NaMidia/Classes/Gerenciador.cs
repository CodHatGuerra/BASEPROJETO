using NaMidiaCore.Linq;
using System.Collections.Generic;

namespace NaMidia.Classes
{
    public class Gerenciador
    {
        public static MainWindow MainWindow { get; set; }
        public static LOGIN UsuarioAtivo { get; set; }

        public static int CodigoModuloAtivo { get; set; }
        public static Dictionary<int, List<int>> DicModuloAcao { get; set; }

        public static bool PermiteInserir { get; set; }
        public static bool PermiteEditar { get; set; }
        public static bool PermiteExcluir { get; set; }
    }
}
