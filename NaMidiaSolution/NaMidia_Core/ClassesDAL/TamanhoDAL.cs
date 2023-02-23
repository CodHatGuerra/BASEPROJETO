using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class TamanhoDAL
    {
        public static void AlterarTamanho(TAMANHO tamanho)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var tamanhoResgatado = dc.TAMANHOs.FirstOrDefault(a => a.cd_Tamanho == tamanho.cd_Tamanho);

                if (tamanhoResgatado != null)
                {
                    tamanhoResgatado.ds_Tamanho = tamanho.ds_Tamanho;
                    tamanhoResgatado.ds_OrdemExibicao = tamanho.ds_OrdemExibicao;
                }

                else
                    dc.TAMANHOs.InsertOnSubmit(tamanho);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirTamanho(int tamanhoId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.TAMANHOs.DeleteOnSubmit(dc.TAMANHOs.FirstOrDefault(a => a.cd_Tamanho == tamanhoId));
                dc.SubmitChanges();
            }
        }

        public static List<TAMANHO> RecuperarListaTamanhos()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.TAMANHOs.OrderBy(a => a.ds_Tamanho).ToList();
        }

        public static TAMANHO RecuperarTamanho(int tamanhoId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.TAMANHOs.FirstOrDefault(a => a.cd_Tamanho == tamanhoId);
        }
    }
}
