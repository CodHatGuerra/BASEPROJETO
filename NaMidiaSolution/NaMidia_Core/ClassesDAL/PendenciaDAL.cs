using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class PendenciaDAL
    {
        public static decimal? valorTotal;
        public static decimal? valorRecebido;

        public static List<PagamentoPendencia> ConsultarPendencias(DateTime dataInicial, DateTime dataFinal)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            var query = (from PV in dc.PAGAMENTOVENDAs
                         where PV.dt_Pagamento_Prevista >= dataInicial && PV.dt_Pagamento_Prevista < dataFinal && PV.statusPagamentoParcela == false
                         select new PagamentoPendencia
                         {
                             nomeCliente = PV.VENDA.PESSOA.nm_Fantasia,
                             dataPrevista = PV.dt_Pagamento_Prevista,
                             valorParcela = PV.ds_ValorReajustado,
                             valorRestante = PV.ds_ValorRestante,
                             cdVenda = PV.cd_Venda
                         }).ToList();

            List<PagamentoPendencia> Lista = new List<PagamentoPendencia>();

            query.ForEach(item =>
            {
                PagamentoPendencia pagamento = new PagamentoPendencia();
                pagamento.nomeCliente = item.nomeCliente;
                pagamento.dataPrevista = item.dataPrevista;

                if (item.valorRestante != null)
                    pagamento.valorParcela = item.valorRestante;
                else
                    pagamento.valorParcela = item.valorParcela;

                pagamento.cdVenda = item.cdVenda;
                Lista.Add(pagamento);
            });

            return Lista;
        }

        public static List<PagamentoPendencia> ConsultarRecebido(DateTime dataInicial, DateTime dataFinal)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            var query = (from PV in dc.PAGAMENTOVENDAs
                         where PV.dt_Pagamento_Efetuado >= dataInicial && PV.dt_Pagamento_Efetuado < dataFinal && PV.ds_ValorRecebido != null
                         select new PagamentoPendencia
                         {
                             nomeCliente = PV.VENDA.PESSOA.nm_Fantasia,
                             dataPrevista = PV.dt_Pagamento_Efetuado,
                             valorParcela = PV.ds_ValorReajustado,
                             valorRecebido = PV.ds_ValorRecebido,
                             cdVenda = PV.cd_Venda
                         }).ToList();

            List<PagamentoPendencia> Lista = new List<PagamentoPendencia>();

            query.ForEach(item =>
            {
                PagamentoPendencia pagamento = new PagamentoPendencia();
                pagamento.nomeCliente = item.nomeCliente;
                pagamento.dataPrevista = item.dataPrevista;
                pagamento.valorParcela = item.valorParcelaReajustado;
                pagamento.valorRecebido = item.valorRecebido;
                pagamento.cdVenda = item.cdVenda;
                Lista.Add(pagamento);
            });

            return Lista;
        }

        public static void CalcularValorTotalRecebido(DateTime dataInicial, DateTime dataFinal)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            valorTotal = (from V in dc.VENDAs
                          join PV in dc.PAGAMENTOVENDAs on V.cd_Venda equals PV.cd_Venda
                          where V.dt_Data >= dataInicial && V.dt_Data < dataFinal
                          select PV.ds_ValorReajustado).Sum();

            valorRecebido = (from PV in dc.PAGAMENTOVENDAs
                             where PV.dt_Pagamento_Efetuado >= dataInicial && PV.dt_Pagamento_Efetuado < dataFinal
                             select PV.ds_ValorRecebido).Sum();
        }
    }

    public class PagamentoPendencia
    {
        public int cdVenda { get; set; }
        public string nomeCliente { get; set; }
        public DateTime? dataPrevista { get; set; }
        public decimal? valorParcela { get; set; }
        public decimal? valorParcelaReajustado { get; set; }
        public decimal? valorRecebido { get; set; }
        public decimal? valorRestante { get; set; }
        public int cdPagamentoVenda { get; set; }
    }
}
