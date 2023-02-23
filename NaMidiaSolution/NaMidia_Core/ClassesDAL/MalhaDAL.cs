using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class MalhaDAL
    {
        public static void AlterarMalha(MALHA malha)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var malhaRecuperada = dc.MALHAs.FirstOrDefault(a => a.cd_Malha == malha.cd_Malha);

                if (malhaRecuperada != null)
                    malhaRecuperada.ds_Malha = malha.ds_Malha;

                else
                    dc.MALHAs.InsertOnSubmit(malha);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirMalha(int malhaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.MALHAs.DeleteOnSubmit(dc.MALHAs.FirstOrDefault(a => a.cd_Malha == malhaId));
                dc.SubmitChanges();
            }
        }

        public static List<MALHA> RecuperarListaMalhas()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.MALHAs.OrderBy(a => a.ds_Malha).ToList();
        }

        public static MALHA RecuperarMalha(int malhaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.MALHAs.Single(a => a.cd_Malha == malhaId);
        }
    }
}
