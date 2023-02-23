using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Reporting.WinForms;
using NaMidiaCore.Linq;
using System.IO;
using NaMidiaCore.ClassesDAL;

using NaMidia.Classes;


namespace NaMidia.RELATORIOS
{
    public partial class HostVenda : Window
    {
        #region [ Propriedades ]

        VENDA venda;
        public VENDA Venda
        {
            get { return venda; }
            set { venda = value; }
        }

        string contato;
        public string Contato
        {
            get { return contato; }
            set { contato = value; }
        }

        List<ItensVenda> listaRelatorio;
        public List<ItensVenda> ListaRelatorio
        {
            get { return listaRelatorio; }
            set { listaRelatorio = value; }
        }

        string fone = string.Empty;
        string email = string.Empty;
        byte[] data;
        string imagem;

        #endregion

        #region [ Construtor ]

        public HostVenda(int vendaId)
        {
            try
            {
                InitializeComponent();

                this.ListaRelatorio = new List<ItensVenda>();

                this.rptViewer.ProcessingMode = ProcessingMode.Local;
                this.rptViewer.LocalReport.ReportPath = Path.Combine(Environment.CurrentDirectory, "Relatorios\\Reports\\RelVenda.rdlc");

                this.rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.rptViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;

                this.Venda = VendaDAL.RecuperarVenda(vendaId);

                var contatoEmailRecuperado = PessoaDAL.RecuperarListaContatoPessoa(Venda.cd_Pessoa).FirstOrDefault(a => a.cd_TipoContato == (int)EnumTipoContato.Email);
                email = contatoEmailRecuperado != null ? contatoEmailRecuperado.ds_Contato : string.Empty; ;

                var contatoFoneRecuperado = PessoaDAL.RecuperarListaContatoPessoa(Venda.cd_Pessoa).FirstOrDefault(a => a.cd_TipoContato != (int)EnumTipoContato.Email);
                Contato = contatoFoneRecuperado != null ? contatoFoneRecuperado.ds_Contato : string.Empty;

                this.data = VendaDAL.RecuperarImagemVenda(Venda.cd_Venda).ds_Imagem.ToArray();
                this.imagem = Convert.ToBase64String(data);

                this.rptViewer.LocalReport.SetParameters(new ReportParameter("cdVenda", vendaId.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeCliente", Venda.PESSOA.nm_Pessoa));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeFantasia", Venda.PESSOA.nm_Fantasia));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Endereco", (Venda.PESSOA.ds_Endereco + (Venda.PESSOA.ds_Numero != null ? "    Nº " + Venda.PESSOA.ds_Numero : string.Empty))));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Bairro", Venda.PESSOA.ds_Bairro));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Cidade", Venda.PESSOA.CIDADE.nm_Cidade));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("imagem", imagem));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Contato", Contato));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("DataEntrega", Venda.dt_DataPrevista.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("ObservacoesVenda", Venda.ds_Observacoes));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Data", Venda.dt_Data.ToString()));

                foreach (var item in VendaDAL.RecuperarListaItensVenda(vendaId).OrderBy(a => a.PRODUTO.ds_OrdemExibicao).ThenBy(a => a.TAMANHO.ds_OrdemExibicao))
                {
                    ItensVenda objeto = new ItensVenda();
                    objeto.Quantidade = item.ds_Quantidade;
                    objeto.Ds_Tamanho = item.TAMANHO.ds_Tamanho;
                    objeto.Ds_Produto = item.PRODUTO.ds_Produto;
                    objeto.Ds_Gola = item.GOLA.ds_Gola;
                    objeto.Ds_Malha = item.MALHA.ds_Malha;
                    objeto.Ds_Observacoes = item.ds_Observacoes;
                    objeto.ValorUnitario = Convert.ToDouble(item.ds_ValorUnitario);
                    objeto.ValorTotal = Convert.ToDouble(item.ds_SubTotal);
                    objeto.Cd_Pedido = item.cd_ItensVenda; // cod itenVenda
                    listaRelatorio.Add(objeto);
                }

                ReportDataSource ds = new ReportDataSource("DataSetVenda", listaRelatorio);
                this.rptViewer.LocalReport.DataSources.Add(ds);
                this.rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [ Eventos UI ]

        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        #endregion
    }
}
