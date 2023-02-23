using System.Windows;
using NaMidiaCore.Linq;
using System.Collections.Generic;

using NaMidia.Classes;
using Telerik.Windows.Controls;
using System.ComponentModel;
using NaMidiaCore.ClassesDAL;
using Telerik.Windows.Controls.GridView;
using System.Windows.Controls;
using System;

namespace NaMidia.UI
{
    public partial class PesquisaClienteUI : RadWindow, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        VendaUI cadastroVenda = new VendaUI();

        PESSOA pessoa;
        public PESSOA Pessoa
        {
            get { return pessoa; }
            set
            {
                pessoa = value;
            }
        }

        private List<PESSOA> listaPessoas;
        public List<PESSOA> ListaPessoas
        {
            get { return PessoaDAL.RecuperarListaPessoa(); }
            set
            {
                this.listaPessoas = value;
            }
        }

        public bool concluiu = false;

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

        public PesquisaClienteUI()
        {
            this.LinhasGridView = 20;

            InitializeComponent();
            this.Loaded += PesquisaClienteUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void PesquisaClienteUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtCliente.RowHeight);

            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }

            Telerik.Windows.Controls.GridViewColumn ColunaClienteFiltro = this.dtCliente.Columns[0];
            FiltroCliente = ColunaClienteFiltro.ColumnFilterDescriptor;
            FiltroCliente.FieldFilter.Filter1.Operator = Telerik.Windows.Data.FilterOperator.Contains;
            FiltroCliente.FieldFilter.Filter1.IsCaseSensitive = false;
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Pessoa != null)
                {
                    if (Pessoa.cd_Pessoa != 0)
                    {
                        this.concluiu = true;
                        this.Close();
                    }

                    else
                        MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dtCliente_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            if (dtCliente.SelectedItem != null)
                Pessoa = dtCliente.SelectedItem as PESSOA;
        }

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
