using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class ProdutoDAL
    {
        public static void AlterarProduto(PRODUTO produto)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var produtoRecuperado = dc.PRODUTOs.FirstOrDefault(a => a.cd_Produto == produto.cd_Produto);

                if (produtoRecuperado != null)
                {
                    produtoRecuperado.ds_Produto = produto.ds_Produto;
                    produtoRecuperado.cd_Categoria = produto.cd_Categoria;
                    produtoRecuperado.cd_Fornecedor = produto.cd_Fornecedor;
                    produtoRecuperado.cd_UnidadeMedida = produto.cd_UnidadeMedida;
                    produtoRecuperado.ds_EstoqueAtual = produto.ds_EstoqueAtual;
                    produtoRecuperado.ds_EstoqueMinimo = produto.ds_EstoqueMinimo;
                    produtoRecuperado.ds_PrecoCusto = produto.ds_PrecoCusto;
                    produtoRecuperado.ds_PrecoVenda = produto.ds_PrecoVenda;
                }

                else
                    dc.PRODUTOs.InsertOnSubmit(produto);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirProduto(int produtoId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.PRODUTOs.DeleteOnSubmit(dc.PRODUTOs.FirstOrDefault(a => a.cd_Produto == produtoId));
                dc.SubmitChanges();
            }
        }

        public static List<PRODUTO> RecuperarListaProdutos()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.PRODUTOs.OrderBy(a => a.ds_Produto).ToList();
        }

        public static PRODUTO RecuperarProduto(int produtoId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.PRODUTOs.FirstOrDefault(a => a.cd_Produto == produtoId);
        }
    }
}
