using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using NaMidia.UI;
using RadGridViewPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NaMidia.RELATORIOS
{
    public partial class RelatorioCostureiraUI : UserControl, INotifyPropertyChanged
    {
        #region [Propriedades]

        public event PropertyChangedEventHandler PropertyChanged;

        List<ViewRelatorioCostureira> listaCostureiraPedido;
        public List<ViewRelatorioCostureira> ListaCostureiraPedido
        {
            get { return CostureiraPedidoDAL.RecuperarListaCostureiraPedido(); }
            set { listaCostureiraPedido = value; }
        }

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

        #region [Construtor]

        public RelatorioCostureiraUI()
        {
            this.LinhasGridView = 20;

            InitializeComponent();
            this.Loaded += RelatorioPagamentoVendaUI_Loaded;
        }

        #endregion

        #region [Eventos]

        private void RelatorioPagamentoVendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtCostureiraPedido.RowHeight);
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dtCostureiraPedido.Columns["columImage"].IsVisible = false;
                dtCostureiraPedido.Columns["columStatusPagamento"].IsVisible = false;

                if (new PrintExportExtensions().ExportRadGridViewToExcel(dtCostureiraPedido))
                    new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();

                dtCostureiraPedido.Columns["columImage"].IsVisible = true;
                dtCostureiraPedido.Columns["columStatusPagamento"].IsVisible = true;
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
