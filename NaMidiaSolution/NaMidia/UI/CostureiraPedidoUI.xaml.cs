using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using NaMidia.RELATORIOS.Hosts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Threading.Tasks;

namespace NaMidia.UI
{
    public partial class CostureiraPedidoUI : UserControl, INotifyPropertyChanged
    {
        #region [Propriedades]

        public event PropertyChangedEventHandler PropertyChanged;

        private VENDA venda;
        public VENDA Venda
        {
            get { return venda; }
            set
            {
                venda = value;
                OnPropertyChanged("Venda");
            }
        }

        private List<VENDA> listaVenda;
        public List<VENDA> ListaVenda
        {
            get { return listaVenda; }
            set
            {
                listaVenda = value;
                OnPropertyChanged("ListaVenda");
            }
        }

        private List<ITENSVENDA> listaItensVenda;
        public List<ITENSVENDA> ListaItensVenda
        {
            get { return listaItensVenda; }
            set
            {
                listaItensVenda = value;
                OnPropertyChanged("ListaItensVenda");
            }
        }

        private double linhasGridView;
        public double LinhasGridView
        {
            get { return linhasGridView; }
            set
            {
                linhasGridView = value;
                OnPropertyChanged("linhasGridView");
            }
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public enum tipoOperacao { atualizarLista, atualizarLista_E_atualizarRegistros }
        tipoOperacao enumTipoOperacao = tipoOperacao.atualizarLista;
        #endregion

        #region [Construtor]

        public CostureiraPedidoUI()
        {
            this.LinhasGridView = 20;
            InitializeComponent();
            this.Loaded += CostureiraPedidoUI_Loaded;
        }

        #endregion

        #region [Eventos]

        private void CostureiraPedidoUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 130) / dtCostureiraPedidoVenda.RowHeight);

            btnExportarPedido.IsEnabled = Gerenciador.PermiteEditar;
            radMenuItem.IsEnabled = Gerenciador.PermiteEditar;

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void dtCostureiraPedidoVenda_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtCostureiraPedidoVenda.SelectedItem != null)
                {
                    this.Venda = this.dtCostureiraPedidoVenda.SelectedItem as VENDA;

                    enumTipoOperacao = tipoOperacao.atualizarLista;
                    this.rbAguarde.IsBusy = true;
                    this.worker.RunWorkerAsync();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RadMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadMenuItem item = (RadMenuItem)sender;
                var itensVendaRecuperado = item.ParentOfType<RadContextMenu>().GetClickedElement<GridViewRow>().Item as ITENSVENDA;

                if (itensVendaRecuperado == null)
                    return;

                var costureiraPedidoRelacaoUI = new CostureiraPedidoRelacaoUI(itensVendaRecuperado.cd_Venda);
                costureiraPedidoRelacaoUI.ShowDialog();

                enumTipoOperacao = tipoOperacao.atualizarLista_E_atualizarRegistros;
                this.rbAguarde.IsBusy = true;
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void rdbAberto_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

                Task.Factory.StartNew(() =>
                {
                    this.ListaVenda = VendaDAL.RecuperarListaVendas().Where(a => a.statusPedidoCostureira == false).ToList();
                }).ContinueWith(task =>
                {
                    if (this.dtCostureiraPedidoVenda != null)
                        this.dtCostureiraPedidoVenda.Rebind();

                    Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void rdbCompleto_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    this.ListaVenda = VendaDAL.RecuperarListaVendas().Where(a => a.statusPedidoCostureira == true).ToList();
                }).ContinueWith(task =>
                {
                    if (this.dtCostureiraPedidoVenda != null)
                        this.dtCostureiraPedidoVenda.Rebind();

                    Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportarPedido_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Venda != null)
                    new HostPedidoCostureira(Venda.cd_Venda).ShowDialog();

                else
                    MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.AtualizarGrid();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.rbAguarde.IsBusy = false;
            }));
        }

        #endregion

        #region [Metodos]

        private void AtualizarGrid()
        {
            switch (enumTipoOperacao)
            {
                case tipoOperacao.atualizarLista:
                    this.ListaItensVenda = VendaDAL.RecuperarListaItensVenda(Venda.cd_Venda).OrderBy(a => a.PRODUTO.ds_OrdemExibicao).ThenBy(a => a.TAMANHO.ds_OrdemExibicao).ToList();
                    break;

                case tipoOperacao.atualizarLista_E_atualizarRegistros:
                    CostureiraPedidoDAL.AtualizarStatusPedidoCostureira(Venda.cd_Venda, (this.VerificarSeTodosItensForamRelacionados(ListaItensVenda)));
                    this.ListaItensVenda = VendaDAL.RecuperarListaItensVenda(Venda.cd_Venda).OrderBy(a => a.PRODUTO.ds_OrdemExibicao).ThenBy(a => a.TAMANHO.ds_OrdemExibicao).ToList();
                    break;
            }
        }

        private bool VerificarSeTodosItensForamRelacionados(List<ITENSVENDA> listaItensCostureiraPedido)
        {
            bool retorno = true;

            foreach (var item in listaItensCostureiraPedido)
            {
                if (CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(item) != 0)
                    retorno = false;
            }

            return retorno;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
