using Microsoft.Win32;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using System;
using System.Windows;
using Telerik.Windows.Controls;
using NaMidiaCore.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using RadGridViewPrint;
using System.Threading.Tasks;

namespace NaMidia.UI
{
    public partial class ConfiguracaoUI : RadWindow, INotifyPropertyChanged
    {
        #region [Propriedade]

        public event PropertyChangedEventHandler PropertyChanged;

        List<ViewLogPagamentoVenda> listaViewLogPagamentoVenda;
        public List<ViewLogPagamentoVenda> ListaViewLogPagamentoVenda
        {
            get { return listaViewLogPagamentoVenda; }
            set
            {
                listaViewLogPagamentoVenda = value;
                OnPropertyChanged("ListaViewLogPagamentoVenda");
            }
        }

        List<ViewLogVenda> listaViewLogVenda;
        public List<ViewLogVenda> ListaViewLogVenda
        {
            get { return listaViewLogVenda; }
            set
            {
                listaViewLogVenda = value;
                OnPropertyChanged("ListaViewLogVenda");
            }
        }

        #endregion

        #region [Construtor]

        public ConfiguracaoUI()
        {
            this.ListaViewLogPagamentoVenda = new List<ViewLogPagamentoVenda>();
            this.ListaViewLogVenda = new List<ViewLogVenda>();

            InitializeComponent();

            Task.Run(() => this.ListaViewLogPagamentoVenda = ConfiguracaoDAL.RecuperarLogsPagamentoVenda()).ContinueWith((t) => Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; })));
        }

        #endregion

        #region [Eventos]

        private void btnImprimirPagamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new PrintExportExtensions().ExportRadGridViewToExcel(dtLogsPagamento))
                    new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimirVenda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new PrintExportExtensions().ExportRadGridViewToExcel(dtLogsVenda))
                    new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tcLogs_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            this.rdAguarde.IsBusy = true;

            if (tcLogs.SelectedIndex == 0)
                Task.Run(() => this.ListaViewLogVenda = ConfiguracaoDAL.RecuperarLogsVenda()).ContinueWith((t) => Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; })));

            else
                Task.Run(() => this.ListaViewLogPagamentoVenda = ConfiguracaoDAL.RecuperarLogsPagamentoVenda()).ContinueWith((t) => Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; })));
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
