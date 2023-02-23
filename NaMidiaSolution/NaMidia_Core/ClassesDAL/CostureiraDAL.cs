using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class CostureiraDAL
    {
        public static void AlterarPessoa(COSTUREIRA costureira, List<CONTATOCOSTUREIRA> listaContato)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var costureiraRecuperada = dc.COSTUREIRAs.FirstOrDefault(a => a.cd_Costureira == costureira.cd_Costureira);

                if (costureiraRecuperada != null)
                {
                    costureiraRecuperada.nm_Costureira = costureira.nm_Costureira;
                    costureiraRecuperada.nm_Fantasia = costureira.nm_Fantasia;
                    costureiraRecuperada.ds_Endereco = costureira.ds_Endereco;
                    costureiraRecuperada.ds_Numero = costureira.ds_Numero;
                    costureiraRecuperada.ds_Bairro = costureira.ds_Bairro;
                    costureiraRecuperada.cd_Cidade = costureira.cd_Cidade;
                    costureiraRecuperada.ds_Cpf = costureira.ds_Cpf;
                    costureiraRecuperada.ds_Rg = costureira.ds_Rg;

                    ExcluirContato(costureira.cd_Costureira);

                    listaContato.ForEach(contato =>
                    {
                        contato.cd_Costureira = costureira.cd_Costureira;
                        dc.CONTATOCOSTUREIRAs.InsertOnSubmit(contato);
                    });
                }

                else
                {
                    dc.COSTUREIRAs.InsertOnSubmit(costureira);

                    listaContato.ForEach(contato =>
                    {
                        contato.COSTUREIRA = costureira;
                        dc.CONTATOCOSTUREIRAs.InsertOnSubmit(contato);
                    });
                }

                dc.SubmitChanges();
            }
        }

        public static void ExcluirCostureira(int costureiraId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                //Apagando todos os contatos relacionados com essa pessoa
                dc.CONTATOCOSTUREIRAs.DeleteAllOnSubmit((from a in dc.CONTATOCOSTUREIRAs
                                                         where a.cd_Costureira == costureiraId
                                                         select a).ToList());

                dc.COSTUREIRAs.DeleteOnSubmit(dc.COSTUREIRAs.FirstOrDefault(a => a.cd_Costureira == costureiraId));
                dc.SubmitChanges();
            }
        }

        private static void ExcluirContato(int costureiraId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CONTATOCOSTUREIRAs.DeleteAllOnSubmit((from a in dc.CONTATOCOSTUREIRAs
                                                         where a.cd_Costureira == costureiraId
                                                         select a));
                dc.SubmitChanges();
            }
        }

        public static void InserirContato(CONTATOCOSTUREIRA contato)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CONTATOCOSTUREIRAs.InsertOnSubmit(contato);
                dc.SubmitChanges();
            }
        }

        public static List<COSTUREIRA> RecuperarListaCostureira()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.COSTUREIRAs.OrderBy(a => a.nm_Costureira).ToList();
        }

        public static List<TIPOCONTATO> RecuperarListaContato()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.TIPOCONTATOs.OrderBy(a => a.nm_TipoContato).ToList();
        }

        public static COSTUREIRA RecuperarCostureira(int costureiraId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.COSTUREIRAs.Single(a => a.cd_Costureira == costureiraId);
        }

        public static List<CONTATOCOSTUREIRA> RecuperarListaContatoPessoa(int costureiraId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.CONTATOCOSTUREIRAs where a.cd_Costureira == costureiraId select a).ToList();
        }
    }
}
