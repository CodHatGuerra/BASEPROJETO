using Microsoft.Reporting.WinForms;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.IO;
using System;

namespace NaMidia.RELATORIOS
{
    public partial class HostPagamentoVenda : Window
    {
        #region [ Propriedades ]
        List<ItensPagamentoVenda> listaPagamentoVenda = new List<ItensPagamentoVenda>();

        PAGAMENTOVENDASREGISTRO pagamentoVendaRegistro;
        public PAGAMENTOVENDASREGISTRO PagamentoVendaRegistro
        {
            get { return pagamentoVendaRegistro; }
            set { pagamentoVendaRegistro = value; }
        }

        #endregion

        #region [ Construtor ]

        public HostPagamentoVenda(int cd_PagamentoRegistro)
        {
            try
            {
                InitializeComponent();

                this.rptViewer.ProcessingMode = ProcessingMode.Local;
                this.rptViewer.LocalReport.ReportPath = Path.Combine(Environment.CurrentDirectory, "Relatorios\\Reports\\RelPagamento.rdlc");

                this.rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.rptViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;

                PagamentoVendaRegistro = VendaDAL.RecuperarPagamentoVendaRegistro(cd_PagamentoRegistro);

                decimal? valorRestanteVenda = VendaDAL.RecuperarParcelasVenda(PagamentoVendaRegistro.PAGAMENTOVENDA.cd_Venda).Sum(a => a.ds_ValorRestante);

                this.rptViewer.LocalReport.SetParameters(new ReportParameter("cdPagamento", PagamentoVendaRegistro.cd_PagamentoVendaRegistros.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeCliente", PagamentoVendaRegistro.PAGAMENTOVENDA.VENDA.PESSOA.nm_Pessoa));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeEmpresa", PagamentoVendaRegistro.PAGAMENTOVENDA.VENDA.PESSOA.nm_Fantasia));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("data", PagamentoVendaRegistro.dt_Pagamento.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("valor", PagamentoVendaRegistro.ds_ValorPagamento.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("restante", valorRestanteVenda.ToString()));

                rptViewer.RefreshReport();
            }

            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
