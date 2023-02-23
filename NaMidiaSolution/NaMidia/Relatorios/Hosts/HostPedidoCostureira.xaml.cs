using Microsoft.Reporting.WinForms;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;

namespace NaMidia.RELATORIOS.Hosts
{
    public partial class HostPedidoCostureira : RadWindow
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
            get { return listaCostureira; }
            set { listaCostureira = value; }
        }

        private int cd_Venda;
        public int Cd_Venda
        {
            get { return cd_Venda; }
            set { cd_Venda = value; }
        }
        #endregion

        #region [Construtor]
        public HostPedidoCostureira(int cd_Venda)
        {
            this.Cd_Venda = cd_Venda;
            this.ListaItensPedidoCostureira = new List<ItensPagamentoCostureira>();
            this.ListaCostureira = CostureiraPedidoDAL.RecuperarListaCostureira(Cd_Venda);

            InitializeComponent();

            // Corrige problema do estilo do WFR
            RadWindowInteropHelper.SetAllowTransparency(this, false);

            this.txtCdVenda.Text = cd_Venda.ToString();
        }
        #endregion

        #region [Metodos]
        public void CarregarReportView()
        {
            try
            {
                this.ListaItensPedidoCostureira.Clear();
                this.rptViewer.ProcessingMode = ProcessingMode.Local;

                this.rptViewer.LocalReport.ReportPath = Path.Combine(Environment.CurrentDirectory, "Relatorios\\Reports\\RelPedidoCostureira.rdlc");

                this.rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.rptViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;

                VENDA Venda = VendaDAL.RecuperarVenda(Cd_Venda);

                var contatoEmailRecuperado = PessoaDAL.RecuperarListaContatoPessoa(Venda.cd_Pessoa).FirstOrDefault(a => a.cd_TipoContato == (int)EnumTipoContato.Email);
                string email = contatoEmailRecuperado != null ? contatoEmailRecuperado.ds_Contato : string.Empty; ;

                var contatoFoneRecuperado = PessoaDAL.RecuperarListaContatoPessoa(Venda.cd_Pessoa).FirstOrDefault(a => a.cd_TipoContato != (int)EnumTipoContato.Email);
                string Contato = contatoFoneRecuperado != null ? contatoFoneRecuperado.ds_Contato : string.Empty;

                byte[] data = VendaDAL.RecuperarImagemVenda(Venda.cd_Venda).ds_Imagem.ToArray();
                string imagem = Convert.ToBase64String(data);

                this.rptViewer.LocalReport.SetParameters(new ReportParameter("cdVenda", cd_Venda.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeCliente", Venda.PESSOA.nm_Pessoa));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeFantasia", Venda.PESSOA.nm_Fantasia));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeCostureira", CostureiraDAL.RecuperarCostureira(Convert.ToInt32(cbCostureira.SelectedValue)).nm_Costureira));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Endereco", (Venda.PESSOA.ds_Endereco + (Venda.PESSOA.ds_Numero != null ? "    Nº " + Venda.PESSOA.ds_Numero : string.Empty))));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Bairro", Venda.PESSOA.ds_Bairro));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Cidade", Venda.PESSOA.CIDADE.nm_Cidade));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("imagem", imagem));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Contato", Contato));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Email", email));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("ObservacoesVenda", Venda.ds_Observacoes));

                foreach (var item in CostureiraPedidoDAL.RecuperarPedido(Convert.ToInt32(cbCostureira.SelectedValue), this.Cd_Venda).ToList())
                {
                    ItensPagamentoCostureira pgc = new ItensPagamentoCostureira();
                    pgc.Ds_Quantidade = (int)item.ds_Quantidade;
                    pgc.Ds_Produto = item.ITENSVENDA.PRODUTO.ds_Produto;
                    pgc.Ds_Tamanho = item.ITENSVENDA.TAMANHO.ds_Tamanho;
                    pgc.Ds_Observação = item.ITENSVENDA.ds_Observacoes;
                    pgc.Ds_Malha = item.ITENSVENDA.MALHA.ds_Malha;
                    pgc.Ds_Gola = item.ITENSVENDA.GOLA.ds_Gola;
                    pgc.Cd_Venda = item.ITENSVENDA.VENDA.cd_Venda;
                    pgc.Ds_ValorUnitario = (decimal)item.ds_ValorUnit;
                    pgc.Ds_ValorTotal = (decimal)item.ds_ValorTotal;

                    ListaItensPedidoCostureira.Add(pgc);
                }

                ReportDataSource ds = new ReportDataSource("DataSetCostureira", ListaItensPedidoCostureira);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(ds);
                rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region [Eventos]
        private void cbCostureira_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregarReportView();
        }
        #endregion
    }
}
