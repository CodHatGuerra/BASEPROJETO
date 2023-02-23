using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class GraficoDAL
    {
        public static List<int> QuantidadeDeVendasPorMes(int ano)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            List<int> lista = new List<int>();
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 1 && a.dt_Data.Year == ano).Count());//Janeiro
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 2 && a.dt_Data.Year == ano).Count()); //Fevereiro
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 3 && a.dt_Data.Year == ano).Count()); //Março
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 4 && a.dt_Data.Year == ano).Count()); //Abril
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 5 && a.dt_Data.Year == ano).Count()); //Maio
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 6 && a.dt_Data.Year == ano).Count()); //Junho
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 7 && a.dt_Data.Year == ano).Count()); //Julho
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 8 && a.dt_Data.Year == ano).Count()); //Agosto
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 9 && a.dt_Data.Year == ano).Count()); //Setembro
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 10 && a.dt_Data.Year == ano).Count()); //Outubro
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 11 && a.dt_Data.Year == ano).Count()); //Novembro
            lista.Add(dc.VENDAs.Where(a => a.dt_Data.Month == 12 && a.dt_Data.Year == ano).Count()); //Dezembro

            return lista;
        }

        public static List<decimal?> ValorDeVendasPorMes(int ano)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            List<decimal?> lista = new List<decimal?>();

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 1 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 2 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 3 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 4 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 5 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 6 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 7 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 8 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 9 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 10 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 11 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            lista.Add((from V in dc.VENDAs
                       join
                           IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                       where V.dt_Data.Month == 12 && V.dt_Data.Year == ano
                       select IV.ds_SubTotal).Sum());

            return lista;

        }

        public static List<Tuple<int, string>> QuantidadeProdutosVendidosPorMes(int produto, int ano)
        {
            List<Tuple<int, string>> listaProdutoMalha = new List<Tuple<int, string>>();

            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                List<ITENSVENDA> listaItensVenda = (from V in dc.VENDAs
                                                    join IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                                                    join M in dc.MALHAs on IV.cd_Malha equals M.cd_Malha
                                                    where IV.cd_Produto == produto && V.dt_Data.Year == ano
                                                    select IV).ToList();

                if (listaItensVenda != null && listaItensVenda.Count != 0)
                {
                    int quantidade = 0;
                    List<int> malha = listaItensVenda.Select(a => a.cd_Malha).Distinct().ToList();

                    foreach (int malhaId in malha)
                    {
                        quantidade = listaItensVenda.Where(a => a.cd_Malha == malhaId).Select(a => a.ds_Quantidade).Sum();
                        listaProdutoMalha.Add(new Tuple<int, string>(quantidade, listaItensVenda.FirstOrDefault(a => a.cd_Malha == malhaId).MALHA.ds_Malha));
                    }
                }
            }

            return listaProdutoMalha;
        }

        public static List<Tuple<int, string>> QuantidadeProdutosVendidosPorMes(int produto, int ano, int mes)
        {
            List<Tuple<int, string>> listaProdutoMalha = new List<Tuple<int, string>>();

            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                List<ITENSVENDA> listaItensVenda = (from V in dc.VENDAs
                                                    join IV in dc.ITENSVENDAs on V.cd_Venda equals IV.cd_Venda
                                                    where IV.cd_Produto == produto && V.dt_Data.Year == ano && V.dt_Data.Month == mes
                                                    select IV).ToList();

                if (listaItensVenda != null && listaItensVenda.Count != 0)
                {
                    int quantidade = 0;
                    List<int> malha = listaItensVenda.Select(a => a.cd_Malha).Distinct().ToList();

                    foreach (int malhaId in malha)
                    {
                        quantidade = listaItensVenda.Where(a => a.cd_Malha == malhaId).Select(a => a.ds_Quantidade).Sum();
                        listaProdutoMalha.Add(new Tuple<int, string>(quantidade, listaItensVenda.FirstOrDefault(a => a.cd_Malha == malhaId).MALHA.ds_Malha));
                    }
                }
            }

            return listaProdutoMalha;
        }
    }
}
