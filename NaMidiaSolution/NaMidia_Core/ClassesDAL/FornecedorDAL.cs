using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class FornecedorDAL
    {
        public static void AlterarPessoa(FORNECEDOR fornecedor, List<CONTATOFORNECEDOR> listaContato)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var fornecedorRecuperado = dc.FORNECEDORs.FirstOrDefault(a => a.cd_Fornecedor == fornecedor.cd_Fornecedor);

                if (fornecedorRecuperado != null)
                {
                    fornecedorRecuperado.nm_Fornecedor = fornecedor.nm_Fornecedor;
                    fornecedorRecuperado.nm_Fantasia = fornecedor.nm_Fantasia;
                    fornecedorRecuperado.ds_Endereco = fornecedor.ds_Endereco;
                    fornecedorRecuperado.ds_Numero = fornecedor.ds_Numero;
                    fornecedorRecuperado.ds_Bairro = fornecedor.ds_Bairro;
                    fornecedorRecuperado.cd_Cidade = fornecedor.cd_Cidade;
                    fornecedorRecuperado.ds_Cpf = fornecedor.ds_Cpf;
                    fornecedorRecuperado.ds_Rg = fornecedor.ds_Rg;

                    ExcluirContato(fornecedor.cd_Fornecedor);

                    listaContato.ForEach(contato =>
                    {
                        contato.cd_Fornecedor = fornecedor.cd_Fornecedor;
                        dc.CONTATOFORNECEDORs.InsertOnSubmit(contato);
                    });
                }

                else
                {
                    dc.FORNECEDORs.InsertOnSubmit(fornecedor);

                    listaContato.ForEach(contato =>
                    {
                        contato.FORNECEDOR = fornecedor;
                        dc.CONTATOFORNECEDORs.InsertOnSubmit(contato);
                    });
                }

                dc.SubmitChanges();
            }
        }

        private static void ExcluirContato(int fornecedorId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CONTATOFORNECEDORs.DeleteAllOnSubmit((from a in dc.CONTATOFORNECEDORs
                                                         where a.cd_Fornecedor == fornecedorId
                                                         select a));
                dc.SubmitChanges();
            }

        }

        public static void ExcluirFornecedor(int fornecedorId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.FORNECEDORs.DeleteOnSubmit(dc.FORNECEDORs.FirstOrDefault(a => a.cd_Fornecedor == fornecedorId));
                dc.SubmitChanges();
            }
        }

        public static FORNECEDOR RecuperarFornecedor(int fornecedorId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.FORNECEDORs.Single(a => a.cd_Fornecedor == fornecedorId);
        }

        public static List<FORNECEDOR> RecuperarListaFornecedor()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.FORNECEDORs.OrderBy(a => a.nm_Fornecedor).ToList();
        }

        public static List<CONTATOFORNECEDOR> RecuperarListaContatoPessoa(int fornecedorId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.CONTATOFORNECEDORs where a.cd_Fornecedor == fornecedorId select a).ToList();
        }
    }
}
