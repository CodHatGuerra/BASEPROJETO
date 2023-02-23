using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class CostureiraPedidoDAL
    {
        public static int RecuperarQuantidadeProdutoDisponivel(ITENSVENDA itensVenda)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                int quantidadeDisp = (from a in dc.COSTUREIRAPEDIDOs where a.cd_ItensVenda == itensVenda.cd_ItensVenda select a.ds_Quantidade).Count() > 0
                    ? (int)(from a in dc.COSTUREIRAPEDIDOs where a.cd_ItensVenda == itensVenda.cd_ItensVenda select a.ds_Quantidade).Sum()
                    : 0;
                return itensVenda.ds_Quantidade > quantidadeDisp ? itensVenda.ds_Quantidade - quantidadeDisp : quantidadeDisp - itensVenda.ds_Quantidade;
            }
        }

        public static List<COSTUREIRAPEDIDO> RecuperarListaCostureiraPedido(int cd_ItensVenda)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<COSTUREIRAPEDIDO>(a => a.COSTUREIRA);
            option.LoadWith<COSTUREIRAPEDIDO>(a => a.ITENSVENDA);
            dc.LoadOptions = option;

            return (from a in dc.COSTUREIRAPEDIDOs where a.ITENSVENDA.cd_ItensVenda == cd_ItensVenda select a).ToList();
        }

        public static void AlterarCostureiraPedido(COSTUREIRAPEDIDO costureiraPedido)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var costureiraPedidoRecuperada = dc.COSTUREIRAPEDIDOs.FirstOrDefault(a => a.cd_CostureiraPedido == costureiraPedido.cd_CostureiraPedido);

                if (costureiraPedidoRecuperada != null)
                {
                    costureiraPedidoRecuperada.cd_Costureira = costureiraPedido.cd_Costureira;
                    costureiraPedidoRecuperada.ds_Quantidade = costureiraPedido.ds_Quantidade;
                    costureiraPedidoRecuperada.ds_ValorUnit = costureiraPedido.ds_ValorUnit;
                    costureiraPedidoRecuperada.ds_ValorTotal = costureiraPedido.ds_ValorTotal;
                    costureiraPedidoRecuperada.ds_Data = costureiraPedido.ds_Data;
                    costureiraPedidoRecuperada.ds_DataPagamento = null;
                    costureiraPedidoRecuperada.statusPagamento = costureiraPedido.statusPagamento;
                }

                else
                    dc.COSTUREIRAPEDIDOs.InsertOnSubmit(costureiraPedido);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirCostureiraPedido(int cd_CostureiraPedido)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.COSTUREIRAPEDIDOs.DeleteOnSubmit(dc.COSTUREIRAPEDIDOs.FirstOrDefault(a => a.cd_CostureiraPedido == cd_CostureiraPedido));
                dc.SubmitChanges();
            }
        }

        public static void AtualizarStatusPedidoCostureira(int vendaId, bool status)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                VENDA vendaRecuperada = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == vendaId);
                vendaRecuperada.statusPedidoCostureira = status;
                dc.SubmitChanges();
            }
        }

        public static List<COSTUREIRAPEDIDO> RecuperarPedidosParaPagamento(int cd_Costureira, DateTime dataInicio, DateTime dateFim)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<COSTUREIRAPEDIDO>(a => a.ITENSVENDA);
            option.LoadWith<COSTUREIRAPEDIDO>(a => a.COSTUREIRA);
            option.LoadWith<ITENSVENDA>(a => a.PRODUTO);
            option.LoadWith<ITENSVENDA>(a => a.TAMANHO);
            option.LoadWith<ITENSVENDA>(a => a.MALHA);
            option.LoadWith<ITENSVENDA>(a => a.GOLA);
            option.LoadWith<ITENSVENDA>(a => a.VENDA);
            option.LoadWith<VENDA>(a => a.PESSOA);
            dc.LoadOptions = option;

            return (from a in dc.COSTUREIRAPEDIDOs
                    where a.ds_Data >= dataInicio &&
                          a.ds_Data <= dateFim.AddDays(1) &&
                          a.cd_Costureira == cd_Costureira
                    select a).ToList();
        }

        public static List<COSTUREIRAPEDIDO> RecuperarPedido(int cd_Costureira, int cd_Venda)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<COSTUREIRAPEDIDO>(a => a.ITENSVENDA);
            option.LoadWith<COSTUREIRAPEDIDO>(a => a.COSTUREIRA);
            option.LoadWith<ITENSVENDA>(a => a.PRODUTO);
            option.LoadWith<ITENSVENDA>(a => a.TAMANHO);
            option.LoadWith<ITENSVENDA>(a => a.MALHA);
            option.LoadWith<ITENSVENDA>(a => a.GOLA);
            option.LoadWith<ITENSVENDA>(a => a.VENDA);
            option.LoadWith<VENDA>(a => a.PESSOA);
            dc.LoadOptions = option;

            return (from a in dc.COSTUREIRAPEDIDOs
                    where a.cd_Costureira == cd_Costureira && a.ITENSVENDA.cd_Venda == cd_Venda
                    orderby a.ITENSVENDA.PRODUTO.ds_OrdemExibicao, a.ITENSVENDA.TAMANHO.ds_OrdemExibicao
                    select a).ToList();
        }

        public static void AlterarPagamento(List<COSTUREIRAPEDIDO> lista)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                lista.ForEach(costureiraPedido =>
                {
                    var costureiraPedidoRecuperada = dc.COSTUREIRAPEDIDOs.FirstOrDefault(a => a.cd_CostureiraPedido == costureiraPedido.cd_CostureiraPedido);
                    costureiraPedidoRecuperada.statusPagamento = true;
                    costureiraPedidoRecuperada.ds_DataPagamento = DateTime.Now;
                });

                dc.SubmitChanges();
            }
        }

        public static void ExcluirPagamentoCostureira(List<COSTUREIRAPEDIDO> lista)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                lista.ForEach(costureiraPedido =>
                {
                    var costureiraPedidoRecuperada = dc.COSTUREIRAPEDIDOs.FirstOrDefault(a => a.cd_CostureiraPedido == costureiraPedido.cd_CostureiraPedido);
                    costureiraPedidoRecuperada.statusPagamento = false;
                    costureiraPedidoRecuperada.ds_DataPagamento = null;
                });

                dc.SubmitChanges();
            }
        }

        public static List<ViewRelatorioCostureira> RecuperarListaCostureiraPedido()
        {
            try
            {
                NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
                return dc.ViewRelatorioCostureiras.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<COSTUREIRA> RecuperarListaCostureira(int vendaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from cp in dc.COSTUREIRAPEDIDOs
                    join
                        c in dc.COSTUREIRAs on cp.cd_Costureira equals c.cd_Costureira
                    where cp.ITENSVENDA.cd_Venda == vendaId
                    select c).Distinct().ToList();
        }
    }
}
