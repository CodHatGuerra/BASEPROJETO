using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class NotificacaoDAL
    {
        public static List<VENDA> RecuperarListaVenda(bool exibirOcultas)
        {
            DateTime diasNotificacao = (DateTime.Today).AddDays(10);

            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            if (!exibirOcultas)
            {
                return (from V in dc.VENDAs
                        where V.dt_DataPrevista <= diasNotificacao && V.statusEntregaVenda == false && V.exibirNotificacao == true
                        orderby V.dt_DataPrevista descending
                        select V).ToList();
            }

            else
            {
                return (from V in dc.VENDAs
                        where V.dt_DataPrevista <= diasNotificacao && V.statusEntregaVenda == false
                        orderby V.dt_DataPrevista descending
                        select V).ToList();
            }
        }

        public static List<PAGAMENTOVENDA> RecuperarListaPagamento(bool exibirOcultas)
        {
            DateTime diasNotificacao = (DateTime.Today).AddDays(10);

            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            if (!exibirOcultas)
            {
                return (from PV in dc.PAGAMENTOVENDAs
                        where PV.dt_Pagamento_Prevista <= diasNotificacao && PV.statusPagamentoParcela == false && PV.exibirNotificacao == true
                        orderby PV.dt_Pagamento_Prevista descending
                        select PV).ToList();
            }

            else
            {
                return (from PV in dc.PAGAMENTOVENDAs
                        where PV.dt_Pagamento_Prevista <= diasNotificacao && PV.statusPagamentoParcela == false
                        orderby PV.dt_Pagamento_Prevista descending
                        select PV).ToList();
            }
        }

        public static void ExcluirNotificacoesVenda(bool excluirTodos, int? cdVenda)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                if (excluirTodos)
                {
                    List<VENDA> listaPagamento = RecuperarListaVenda(false);

                    listaPagamento.ForEach(item =>
                    {
                        var venda = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == item.cd_Venda);

                        if (venda != null)
                        {
                            venda.exibirNotificacao = false;
                            dc.SubmitChanges();
                        }
                    });
                }

                else
                {
                    var venda = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == cdVenda);

                    if (venda != null)
                    {
                        venda.exibirNotificacao = false;
                        dc.SubmitChanges();
                    }
                }
            }
        }

        public static void ExcluirNotificacoesPagamento(bool excluirTodos, int? cdPagamentoVenda)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                if (excluirTodos)
                {
                    List<PAGAMENTOVENDA> listaPagamento = RecuperarListaPagamento(false);

                    listaPagamento.ForEach(pagamentoVenda => 
                    {
                        var pagamento = dc.PAGAMENTOVENDAs.FirstOrDefault(a => a.cd_PagamentoVenda == pagamentoVenda.cd_PagamentoVenda);

                        if (pagamento != null)
                        {
                            pagamento.exibirNotificacao = false;
                            dc.SubmitChanges();
                        }
                    });
                }

                else
                {
                    var pagamento = dc.PAGAMENTOVENDAs.FirstOrDefault(a => a.cd_PagamentoVenda == cdPagamentoVenda);

                    if (pagamento != null)
                    {
                        pagamento.exibirNotificacao = false;
                        dc.SubmitChanges();
                    }
                }
            }
        }
    }
}
