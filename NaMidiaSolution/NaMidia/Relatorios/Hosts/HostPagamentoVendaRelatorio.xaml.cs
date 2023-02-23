using Microsoft.Reporting.WinForms;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;

using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace NaMidia.Relatorios.Hosts
{
    public partial class HostPagamentoVendaRelatorio : Window
    {
        #region [ Propriedades ]
        List<ItensPagamentoVenda> listaPagamentoVenda = new List<ItensPagamentoVenda>();

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

        string email;
        #endregion

        #region [ Construtor ]

        public HostPagamentoVendaRelatorio(int cd_Venda)
        {
            try
            {
                InitializeComponent();

                this.rptViewer.ProcessingMode = ProcessingMode.Local;
                this.rptViewer.LocalReport.ReportPath = Path.Combine(Environment.CurrentDirectory, "Relatorios\\Reports\\RelPagamentoVenda.rdlc");

                this.rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.rptViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;

                Venda = VendaDAL.RecuperarVenda(cd_Venda);

                var contatoEmailRecuperado = PessoaDAL.RecuperarListaContatoPessoa(Venda.cd_Pessoa).FirstOrDefault(a => a.cd_TipoContato == (int)EnumTipoContato.Email);
                email = contatoEmailRecuperado != null ? contatoEmailRecuperado.ds_Contato : string.Empty; ;

                var contatoFoneRecuperado = PessoaDAL.RecuperarListaContatoPessoa(Venda.cd_Pessoa).FirstOrDefault(a => a.cd_TipoContato != (int)EnumTipoContato.Email);
                Contato = contatoFoneRecuperado != null ? contatoFoneRecuperado.ds_Contato : string.Empty;

                this.rptViewer.LocalReport.SetParameters(new ReportParameter("cdVenda", cd_Venda.ToString()));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeCliente", Venda.PESSOA.nm_Pessoa));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("nomeEmpresa", Venda.PESSOA.nm_Fantasia));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("endereco", (Venda.PESSOA.ds_Endereco + (Venda.PESSOA.ds_Numero != null ? "    Nº " + Venda.PESSOA.ds_Numero : string.Empty))));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("bairro", Venda.PESSOA.ds_Bairro));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("cidade", Venda.PESSOA.CIDADE.nm_Cidade));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("Contato", Contato));
                this.rptViewer.LocalReport.SetParameters(new ReportParameter("email", email));

                foreach (var pagamentoVenda in VendaDAL.RecuperarListaPagamentoVenda(Venda.cd_Venda))
                {
                    ItensPagamentoVenda pgv = new ItensPagamentoVenda();
                    pgv.DataPagamento = null;
                    pgv.DataPrevista = pagamentoVenda.dt_Pagamento_Prevista;
                    pgv.Ds_Reajuste = pagamentoVenda.REAJUSTE.ds_Reajuste;
                    pgv.ValorParcela = pagamentoVenda.ds_ValorParcela;
                    pgv.ValorReajuste = pagamentoVenda.ds_ValorReajuste;
                    pgv.ValorRecebido = pagamentoVenda.ds_ValorRecebido;
                    pgv.ValorRestante = pagamentoVenda.ds_ValorRestante;
                    listaPagamentoVenda.Add(pgv);

                    foreach (var pagamentoVendaRegistro in VendaDAL.RecuperPagamentosParcela(pagamentoVenda.cd_PagamentoVenda))
                    {
                        ItensPagamentoVenda pgvr = new ItensPagamentoVenda();
                        pgvr.ValorRecebido = pagamentoVendaRegistro.ds_ValorPagamento;
                        pgvr.DataPagamento = pagamentoVendaRegistro.dt_Pagamento;
                        listaPagamentoVenda.Add(pgvr);
                    }
                    listaPagamentoVenda.Add(null);
                }

                ReportDataSource ds = new ReportDataSource("DataSetPagamento", listaPagamentoVenda);
                rptViewer.LocalReport.DataSources.Add(ds);
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
