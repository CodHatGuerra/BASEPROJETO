using NaMidiaCore.Linq;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class ParametroSistemaDAL
    {
        public static string RecuperarParametroSistema(int parametroSistemaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.PARAMETROSISTEMAs.FirstOrDefault(a => a.cd_ParametroSistema == parametroSistemaId).ds_Valor;
        }
    }
}
