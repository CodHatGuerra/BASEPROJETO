using Microsoft.Reporting.WinForms;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace NaMidia.RELATORIOS.Hosts
{
    public partial class HostPagamentoCostureira : Window
    {
        #region [Propriedades]
        private List<ItensPagamentoCostureira> listaItensPedidoCostureira;
        public List<ItensPagamentoCostureira> ListaItensPedidoCostureira
        {
            get { return listaItensPedidoCostureira; }
            set
            {
                listaItensPedidoCostureira = value;
            }
        }

        private List<COSTUREIRA> listaCostureira;
        public List<COSTUREIRA> ListaCostureira
        {
            get { return CostureiraDAL.RecuperarListaCostureira(); }
            set { listaCostureira = value; }
        }

        private int cd_Venda;
        public int Cd_Venda
        {
            get { return cd_Venda; }
            set { cd_Venda = value; }
        }
        #endregion

        public HostPagamentoCostureira(List<COSTUREIRAPEDIDO> listaCostureiraPedido)
        {
            try
            {
                InitializeComponent();

                ListaItensPedidoCostureira = new List<ItensPagamentoCostureira>();


                List<int> cd_Vendas = (from a in listaCostureiraPedido
                                       select a.ITENSVENDA.cd_Venda).Distinct().ToList();

                foreach (var item in cd_Vendas)
                {
                    ItensPagamentoCostureira itens = new ItensPagamentoCostureira();
                    itens.Cd_Venda = item;
                    itens.Nm_Costureira = listaCostureiraPedido.FirstOrDefault(a => a.ITENSVENDA.cd_Venda == item).COSTUREIRA.nm_Costureira;
                    itens.Nm_Cliente = (string)listaCostureiraPedido.FirstOrDefault(a => a.ITENSVENDA.cd_Venda == item).ITENSVENDA.VENDA.PESSOA.nm_Fantasia;
                    itens.Ds_ValorTotal = (decimal)listaCostureiraPedido.Where(a => a.ITENSVENDA.cd_Venda == item).Sum(a => a.ds_ValorTotal);
                    itens.Ds_DataPedido = (DateTime)listaCostureiraPedido.FirstOrDefault(a => a.ITENSVENDA.cd_Venda == item).ds_Data;
                    ListaItensPedidoCostureira.Add(itens);
                }

                this.rptViewer.ProcessingMode = ProcessingMode.Local;
                this.rptViewer.LocalReport.ReportPath = Path.Combine(Environment.CurrentDirectory, "Relatorios\\Reports\\RelPagamentoCostureira.rdlc");

                this.rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.rptViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;

                ReportDataSource ds = new ReportDataSource("DataSetPagamentoCostureira", ListaItensPedidoCostureira);
                rptViewer.LocalReport.DataSources.Add(ds);
                rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
