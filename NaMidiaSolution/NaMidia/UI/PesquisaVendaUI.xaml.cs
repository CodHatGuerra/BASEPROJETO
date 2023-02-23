using NaMidiaCore.ClassesDAL;
using System;
using System.Windows;
using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using NaMidia.Classes;
using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Windows.Controls;

namespace NaMidia.UI
{
    public partial class PesquisaVendaUI : RadWindow, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        List<VENDA> listaVenda;
        public List<VENDA> ListaVenda
        {
            get { return listaVenda; }
            set
            {
                listaVenda = value;
                OnPropertyChanged("listaVenda");
            }
        }

        VENDA venda;
        public VENDA Venda
        {
            get { return venda; }
            set
            {
                venda = value;
                OnPropertyChanged("venda");
            }
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

        IColumnFilterDescriptor FiltroCliente { get; set; }
        TextBox txtFiltroCliente { get; set; }
        #endregion

        #region [ Construtor ]

        public PesquisaVendaUI()
        {
            this.ListaVenda = VendaDAL.RecuperarListaVendasOrcamento().ToList();
            this.Venda = new VENDA();
            this.LinhasGridView = 20;

            InitializeComponent();
            this.Loaded += PesquisaVendaUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void PesquisaVendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtVenda.RowHeight);

            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }

            Telerik.Windows.Controls.GridViewColumn ColunaClienteFiltro = this.dtVenda.Columns[1];
            FiltroCliente = ColunaClienteFiltro.ColumnFilterDescriptor;
            FiltroCliente.FieldFilter.Filter1.Operator = Telerik.Windows.Data.FilterOperator.Contains;
            FiltroCliente.FieldFilter.Filter1.IsCaseSensitive = false;
        }

        #region [RadGridView]

        private void dtVenda_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtVenda.SelectedItem != null)
                    Venda = VendaDAL.RecuperarVenda((dtVenda.SelectedItem as VENDA).cd_Venda);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [Botoes]

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (Venda.cd_Venda != 0)
                this.Close();

            else
                MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            Venda = new VENDA();
            this.Close();
        }

        #endregion

        private void txtFiltroCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var text = (sender as TextBox);

                if (txtFiltroCliente == null)
                    txtFiltroCliente = text;

                if (FiltroCliente != null)
                    FiltroCliente.FieldFilter.Filter1.Value = txtFiltroCliente.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region [ Metodos ]

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
