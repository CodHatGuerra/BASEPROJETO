using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class GolaDAL
    {
        public static void AlterarGola(GOLA gola)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var golaRecuperada = dc.GOLAs.FirstOrDefault(a => a.cd_Gola == gola.cd_Gola);

                if (golaRecuperada != null)
                    golaRecuperada.ds_Gola = gola.ds_Gola;

                else
                    dc.GOLAs.InsertOnSubmit(gola);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirGola(int golaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.GOLAs.DeleteOnSubmit(dc.GOLAs.FirstOrDefault(a => a.cd_Gola == golaId));
                dc.SubmitChanges();
            }
        }

        public static List<GOLA> RecuperarListaGolas()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.GOLAs.OrderBy(a => a.ds_Gola).ToList();
        }

        public static GOLA RecuperarGola(int golaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.GOLAs.Single(a => a.cd_Gola == golaId);
        }
    }
}
