using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class SistemaDAL
    {
        public static List<MODULO> RecuperarListaModulo(int codigoUsuario)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                List<MODULO> lstModulo = new List<MODULO>();
                var lista = dc.ProcRecuperarModulo(codigoUsuario);

                foreach (var modulo in lista)
                {
                    lstModulo.Add(new MODULO
                    {
                        cd_Modulo = modulo.cd_Modulo,
                        cd_ModuloPai = modulo.cd_ModuloPai,
                        ds_Modulo = modulo.ds_Modulo,
                        ds_OrdemExibicao = modulo.ds_OrdemExibicao,
                        ds_Icon = modulo.ds_Icon
                    });
                }
                return lstModulo;
            }
        }

        public static List<ProcRecuperarModuloAcaoResult> RecuperarListaModuloAcao(int codigoUsuario, int codigoModulo)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.ProcRecuperarModuloAcao(codigoUsuario, codigoModulo).ToList();
        }
    }
}
