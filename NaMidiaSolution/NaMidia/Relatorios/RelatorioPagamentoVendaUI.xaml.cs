using System;
using System.Windows;
using System.Windows.Controls;
using NaMidiaCore.Linq;
using NaMidiaCore.ClassesDAL;
using NaMidia.Classes;
using System.Collections.Generic;
using RadGridViewPrint;
using NaMidia.UI;
using Telerik.Windows.Controls;
using NaMidia.Relatorios.Hosts;
using System.ComponentModel;

namespace NaMidia.RELATORIOS
{
    public partial class RelatorioPagamentoVendaUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        List<ViewRelatorioPagamento> listaPagamentosVenda;
        public List<ViewRelatorioPagamento> ListaPagamentosVenda
        {
            get { return VendaDAL.RecuperarListaPagamentoVenda(); }
            set { listaPagamentosVenda = value; }
        }

        List<PAGAMENTOVENDASREGISTRO> listaPagamentoVendaRegistro;
        public List<PAGAMENTOVENDASREGISTRO> ListaPagamentoVendaRegistro
        {
            get { return listaPagamentoVendaRegistro; }
            set
            {
                listaPagamentoVendaRegistro = value;
            }
        }

        ViewRelatorioPagamento viewRelatorioPagamento = new ViewRelatorioPagamento();

        RadGridView radGridViewParcela;

        private double linhasGridView;
        public double LinhasGridView
        {
            get { return linhasGridView; }
            set
            {
                linhasGridView = value;
                OnPropertyChanged("LinhasGridView");
            }
        }

        #endregion

        #region [ Construtor ]

        public RelatorioPagamentoVendaUI()
        {
            this.LinhasGridView = 20;

            InitializeComponent();
            this.Loaded += RelatorioPagamentoVendaUI_Loaded;
        }

        #endregion

        #region [ Eventos UI ]

        private void RelatorioPagamentoVendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtPagamentoVenda.RowHeight);
        }

        #region [RadGridView]

        private void dtPagamentoVenda_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtPagamentoVenda.SelectedItem != null)
                    this.viewRelatorioPagamento = dtPagamentoVenda.SelectedItem as ViewRelatorioPagamento;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtParcela_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                radGridViewParcela = (RadGridView)sender;
                ListaPagamentoVendaRegistro = VendaDAL.RecuperPagamentosParcela(viewRelatorioPagamento.cd_PagamentoVenda);
                radGridViewParcela.ItemsSource = ListaPagamentoVendaRegistro;
                radGridViewParcela.Rebind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtPagamentoVenda_RowDetailsVisibilityChanging(object sender, Telerik.Windows.Controls.GridView.RowDetailsVisibilityChangingEventArgs e)
        {
            if (e.Row != null)
                e.Row.IsSelected = true;
        }

        #endregion

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new OpcoesExportacaoUI().ShowDialog();

                if (OpcoesExportacaoUI.tipoExportacao == EnumTipoExportacao.ExportarSelecionado)
                {
                    if (viewRelatorioPagamento.cd_Venda != 0)
                        new HostPagamentoVendaRelatorio(viewRelatorioPagamento.cd_Venda).ShowDialog();

                    else
                        MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                else if (OpcoesExportacaoUI.tipoExportacao == EnumTipoExportacao.ExportarTodosExcel)
                {
                    if (new PrintExportExtensions().ExportRadGridViewToExcel(dtPagamentoVenda))
                        new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [Metodos]

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
