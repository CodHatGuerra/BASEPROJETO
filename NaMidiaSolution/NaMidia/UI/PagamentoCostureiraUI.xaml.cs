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

namespace NaMidia.UI
{
    public partial class PagamentoCostureiraUI : UserControl, INotifyPropertyChanged
    {
        #region [Propriedades]

        public event PropertyChangedEventHandler PropertyChanged;

        private List<COSTUREIRAPEDIDO> listaCostureiraPedido;
        public List<COSTUREIRAPEDIDO> ListaCostureiraPedido
        {
            get { return listaCostureiraPedido; }
            set
            {
                listaCostureiraPedido = value;
                OnPropertyChanged("ListaCostureiraPedido");
            }
        }

        private List<COSTUREIRAPEDIDO> listaCostureiraPedidoPagamento;
        public List<COSTUREIRAPEDIDO> ListaCostureiraPedidoPagamento
        {
            get { return listaCostureiraPedidoPagamento; }
            set
            {
                listaCostureiraPedidoPagamento = value;
                OnPropertyChanged("ListaCostureiraPedidoPagamento");
            }
        }

        private List<COSTUREIRA> listaCostureira;
        public List<COSTUREIRA> ListaCostureira
        {
            get { return CostureiraDAL.RecuperarListaCostureira(); }
            set
            {
                listaCostureira = value;
                OnPropertyChanged("ListaCostureira");
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

        #endregion

        #region [Construtor]

        public PagamentoCostureiraUI()
        {
            this.LinhasGridView = 20;

            this.ListaCostureiraPedido = new List<COSTUREIRAPEDIDO>();
            this.ListaCostureiraPedidoPagamento = new List<COSTUREIRAPEDIDO>();

            InitializeComponent();
            this.Loaded += PagamentoCostureiraUI_Loaded;
        }

        #endregion

        #region [Eventos]

        private void PagamentoCostureiraUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtPagamentoCostureira.RowHeight);
            dpPagamento.SelectedDate = DateTime.Now;
            dpPagamentoFim.SelectedDate = DateTime.Now;

            this.btnSalvar.IsEnabled = Gerenciador.PermiteEditar;
            this.btnAdd.IsEnabled = Gerenciador.PermiteEditar;
            this.btnRemover.IsEnabled = Gerenciador.PermiteEditar;
            this.btnExportarPedido.IsEnabled = Gerenciador.PermiteEditar;
            this.btnExportarPedidoPagamento.IsEnabled = Gerenciador.PermiteEditar;
            this.rmExcluirPagamento.IsEnabled = Gerenciador.PermiteEditar;
        }

        #region [Botoes]

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CarregarPagamentos(Convert.ToInt32(cbCostureira.SelectedValue), dpPagamento.SelectedDate.Value, dpPagamentoFim.SelectedDate.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEfetuarPagamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListaCostureiraPedidoPagamento.Count > 0)
                {
                    CostureiraPedidoDAL.AlterarPagamento(ListaCostureiraPedidoPagamento);
                    this.CarregarPagamentos(Convert.ToInt32(cbCostureira.SelectedValue), dpPagamento.SelectedDate.Value, dpPagamentoFim.SelectedDate.Value);
                }

                else
                    MessageBox.Show(Mensagens.faltamInformacoes, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ListaCostureiraPedidoPagamento != null)
                {
                    this.ListaCostureiraPedidoPagamento.Clear();
                    this.dtPagamentoCostureiraPagamento.Rebind();
                }

                if (this.ListaCostureiraPedido != null)
                {
                    this.ListaCostureiraPedido.Clear();
                    this.dtPagamentoCostureira.Rebind();
                }

                this.dpPagamento.SelectedDate = DateTime.Now;
                this.dpPagamentoFim.SelectedDate = DateTime.Now;

                this.cbCostureira.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListaCostureiraPedido.Count > 0)
                new HostPagamentoCostureira(this.ListaCostureiraPedido).ShowDialog();
            else
                MessageBox.Show(Mensagens.faltamInformacoes, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExportarPedidoPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListaCostureiraPedidoPagamento.Count > 0)
                new HostPagamentoCostureira(this.ListaCostureiraPedidoPagamento).ShowDialog();
            else
                MessageBox.Show(Mensagens.faltamInformacoes, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool atualizarRadGridView = false;
                bool aceitarSubstituirDataPagamento = false;

                COSTUREIRAPEDIDO costureiraPedido = dtPagamentoCostureira.SelectedItem as COSTUREIRAPEDIDO;

                if (costureiraPedido != null)
                {
                    foreach (var item in this.ListaCostureiraPedido.Where(a => a.ITENSVENDA.cd_Venda == costureiraPedido.ITENSVENDA.cd_Venda).ToList())
                    {
                        if (item.statusPagamento == true && aceitarSubstituirDataPagamento == false)
                        {
                            if (MessageBox.Show(Mensagens.desejaPagarItensCostureiraJaPagos, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                aceitarSubstituirDataPagamento = true;
                                this.EditarListasPagamentoCostureira(true, item);
                                atualizarRadGridView = true;
                            }
                            else
                                break;
                        }

                        else
                        {
                            this.EditarListasPagamentoCostureira(true, item);
                            atualizarRadGridView = true;
                        }
                    }

                    if (atualizarRadGridView)
                        this.AtualizarRadGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemover_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool atualizarRadGridView = false;

                COSTUREIRAPEDIDO costureiraPedido = dtPagamentoCostureiraPagamento.SelectedItem as COSTUREIRAPEDIDO;
                if (costureiraPedido != null)
                {
                    foreach (var item in this.ListaCostureiraPedidoPagamento.Where(a => a.ITENSVENDA.cd_Venda == costureiraPedido.ITENSVENDA.cd_Venda).ToList())
                    {
                        this.EditarListasPagamentoCostureira(false, item);
                        atualizarRadGridView = true;
                    }

                    if (atualizarRadGridView)
                        this.AtualizarRadGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void dtPagamentoCostureira_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var s = e.OriginalSource as FrameworkElement;

                if (s is TextBlock)
                {
                    var parentRow = s.ParentOfType<GridViewRow>();

                    if (parentRow != null)
                        parentRow.IsSelected = true;
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
                if (MessageBox.Show(Mensagens.desejaExcluirPagamentoCostureira, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    COSTUREIRAPEDIDO costureiraPedido = dtPagamentoCostureira.SelectedItem as COSTUREIRAPEDIDO;

                    if (costureiraPedido != null)
                    {
                        CostureiraPedidoDAL.ExcluirPagamentoCostureira(this.ListaCostureiraPedido.Where(a => a.ITENSVENDA.cd_Venda == costureiraPedido.ITENSVENDA.cd_Venda).ToList());
                        this.CarregarPagamentos(Convert.ToInt32(cbCostureira.SelectedValue), dpPagamento.SelectedDate.Value, dpPagamentoFim.SelectedDate.Value);
                    }
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

        private void EditarListasPagamentoCostureira(bool adicionarPagamento, COSTUREIRAPEDIDO item)
        {
            switch (adicionarPagamento)
            {
                case true:
                    this.ListaCostureiraPedido.Remove(this.ListaCostureiraPedido.FirstOrDefault(a => a.cd_CostureiraPedido == item.cd_CostureiraPedido));
                    this.ListaCostureiraPedidoPagamento.Add(item);
                    break;

                case false:
                    this.ListaCostureiraPedidoPagamento.Remove(this.ListaCostureiraPedidoPagamento.FirstOrDefault(a => a.cd_CostureiraPedido == item.cd_CostureiraPedido));
                    this.ListaCostureiraPedido.Add(item);
                    break;
            }
        }

        private void AtualizarRadGridView()
        {
            this.dtPagamentoCostureira.Rebind();
            this.dtPagamentoCostureiraPagamento.Rebind();
        }

        private void CarregarPagamentos(int costureiraId, DateTime dataInicio, DateTime dataFim)
        {
            if (this.ListaCostureiraPedido != null)
                this.ListaCostureiraPedido.Clear();

            if (this.ListaCostureiraPedidoPagamento != null)
                this.ListaCostureiraPedidoPagamento.Clear();

            foreach (COSTUREIRAPEDIDO costureiraPedidoItem in CostureiraPedidoDAL.RecuperarPedidosParaPagamento(costureiraId, dataInicio, dataFim))
            {
                if (costureiraPedidoItem.statusPagamento == true)
                    this.ListaCostureiraPedido.Add(costureiraPedidoItem);
                else
                    this.ListaCostureiraPedidoPagamento.Add(costureiraPedidoItem);
            }

            this.AtualizarRadGridView();
        }

        #endregion
    }
}
