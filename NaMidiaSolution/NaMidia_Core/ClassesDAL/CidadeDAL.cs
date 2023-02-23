using System.Collections.Generic;
using System.Linq;
using NaMidiaCore.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class CidadeDAL
    {
        public static void AlterarCidade(CIDADE cidade)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var cidadeRecuperada = dc.CIDADEs.FirstOrDefault(a => a.cd_Cidade == cidade.cd_Cidade);

                if (cidadeRecuperada != null)
                {
                    cidadeRecuperada.nm_Cidade = cidade.nm_Cidade;
                    cidadeRecuperada.cd_Estado = cidade.cd_Estado;
                }

                else
                    dc.CIDADEs.InsertOnSubmit(cidade);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirCidade(int cidadeId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CIDADEs.DeleteOnSubmit(dc.CIDADEs.FirstOrDefault(a => a.cd_Cidade == cidadeId));
                dc.SubmitChanges();
            }
        }

        public static CIDADE RecuperarCidade(int cidadeId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.CIDADEs.Single(a => a.cd_Cidade == cidadeId);
        }

        public static List<CIDADE> RecuperarListaCidade()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.CIDADEs.OrderBy(a => a.nm_Cidade).ToList();
        }

        public static List<UF> RecuperarListaUF()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.UFs.OrderBy(a => a.nm_Uf).ToList();
        }
    }
}
