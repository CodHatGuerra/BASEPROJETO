using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using RadGridViewPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace NaMidia.UI
{
    public partial class VendaEntregaUI : UserControl, INotifyPropertyChanged
    {
        #region [Propriedades]

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

        List<VENDA> listaVendaAux;
        public List<VENDA> ListaVendaAux
        {
            get { return listaVendaAux; }
            set
            {
                listaVendaAux = value;
                this.OnPropertyChanged("ListaVendaAux");
            }
        }

        List<VENDA> listaVenda;
        public List<VENDA> ListaVenda
        {
            get { return listaVenda; }
            set
            {
                listaVenda = value;
                this.OnPropertyChanged("ListaVenda");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [Construtor]

        public VendaEntregaUI()
        {
            this.ListaVendaAux = new List<VENDA>();
            LinhasGridView = 20;
            InitializeComponent();
        }

        #endregion

        #region [Eventos]

        private void ucVendaEntregueUI_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaVendaAux = VendaDAL.RecuperarListaVendas().ToList();

            }).ContinueWith(task =>
            {
                this.LinhasGridView = (double)((rGridView.ActualHeight - 150) / dtVenda.RowHeight);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;

                radMenuItem.IsEnabled = Gerenciador.PermiteEditar;
                btnImprimir.IsEnabled = Gerenciador.PermiteEditar;
                CarregarListaVenda(rdbEntregue.IsChecked == true ? true : false);

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region [RadGridView]

        private void RadContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (rdbEntregue.IsChecked == true)
                radMenuItem.Header = "Desmarcar Entrega";

            else
                radMenuItem.Header = "Marcar Entrega";
        }

        private void RadMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadMenuItem item = (RadMenuItem)sender;
                var v = item.ParentOfType<RadContextMenu>().GetClickedElement<GridViewRow>();

                if (v != null)
                {
                    VENDA venda = v.Item as VENDA;

                    if (radMenuItem.Header.ToString() == "Marcar Entrega")
                    {
                        PesquisaDataEntregaVenda pesquisaDataEntregaVenda = new PesquisaDataEntregaVenda(venda.cd_Venda, true);
                        pesquisaDataEntregaVenda.ShowDialog();

                        if (pesquisaDataEntregaVenda.concluiu)
                            this.CarregarListaVenda(rdbEntregue.IsChecked == true ? true : false);
                    }
                    else
                    {
                        if (MessageBox.Show(Mensagens.desmarcarDataEntrega, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            PesquisaDataEntregaVenda pesquisaDataEntregaVenda = new PesquisaDataEntregaVenda(venda.cd_Venda, false);
                            pesquisaDataEntregaVenda.AlterarDataEntregaVenda();

                            if (pesquisaDataEntregaVenda.concluiu)
                                this.CarregarListaVenda(rdbEntregue.IsChecked == true ? true : false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [RadioButton]

        private void rdbEntregue_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CarregarListaVenda(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void rdbNaoEntregue_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CarregarListaVenda(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new PrintExportExtensions().ExportRadGridViewToExcel(dtVenda))
                    new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();
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

        public void CarregarListaVenda(bool estadoEntrega)
        {
            this.ListaVenda = this.ListaVendaAux.Where(a => a.statusEntregaVenda == estadoEntrega).ToList();
        }

        #endregion
    }
}
