using NaMidia.Classes;
using NaMidia.RELATORIOS;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace NaMidia.UI
{
    public partial class PagamentoVendaUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        VENDA venda;
        public VENDA Venda
        {
            get { return venda; }
            set
            {
                venda = value;
                this.OnPropertyChanged("Venda");
            }
        }

        PAGAMENTOVENDA pagamentoVenda;
        public PAGAMENTOVENDA PagamentoVenda
        {
            get { return pagamentoVenda; }
            set
            {
                pagamentoVenda = value;
                this.OnPropertyChanged("PagamentoVenda");
            }
        }

        PAGAMENTOVENDASREGISTRO pagamentoVendaRegistro;
        public PAGAMENTOVENDASREGISTRO PagamentoVendaRegistro
        {
            get { return pagamentoVendaRegistro; }
            set
            {
                pagamentoVendaRegistro = value;
                this.OnPropertyChanged("PagamentoVendaRegistro");
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

        List<PAGAMENTOVENDA> listaPagamentoVenda;
        public List<PAGAMENTOVENDA> ListaPagamentoVenda
        {
            get { return listaPagamentoVenda; }
            set
            {
                listaPagamentoVenda = value;
                this.OnPropertyChanged("ListaPagamentoVenda");
            }
        }

        List<PAGAMENTOVENDASREGISTRO> listaPagamentoVendaRegistro;
        public List<PAGAMENTOVENDASREGISTRO> ListaPagamentoVendaRegistro
        {
            get { return listaPagamentoVendaRegistro; }
            set
            {
                listaPagamentoVendaRegistro = value;
                this.OnPropertyChanged("ListaPagamentoVendaRegistro");
            }
        }

        private int linhasGridView;
        public int LinhasGridView
        {
            get { return linhasGridView; }
            set
            {
                linhasGridView = value;
                OnPropertyChanged("LinhasGridView");
            }
        }

        #region [Propriedades Pagamento Parcela da Parcela]
        RadTabControl radTabControlTemplate;
        TextBox txtValorPagoParcela;
        TextBox txtValorRestanteParcela;
        RadButton btnSalvarParcela;
        RadButton btnCancelarParcela;
        RadGridView radGridViewParcela;
        #endregion


        EnumTipoOperacao enumTipoOperacao;

        decimal? valorTotal = 0;
        public decimal? ValorTotal
        {
            get { return valorTotal; }
            set
            {
                valorTotal = value;
                this.OnPropertyChanged("ValorTotal");
            }
        }

        decimal? valorTotalComReajuste = 0;
        public decimal? ValorTotalComReajuste
        {
            get { return valorTotalComReajuste; }
            set
            {
                valorTotalComReajuste = value;
                this.OnPropertyChanged("ValorTotalComReajuste");
            }
        }

        decimal? valorPago = 0;
        public decimal? ValorPago
        {
            get { return valorPago; }
            set
            {
                valorPago = value;
                this.OnPropertyChanged("ValorPago");
            }
        }

        decimal? valorParcela = 0;
        public decimal? ValorParcela
        {
            get { return valorParcela; }
            set
            {
                valorParcela = value;
                this.OnPropertyChanged("ValorParcela");
            }
        }

        decimal? valorRestante;
        public decimal? ValorRestante
        {
            get { return valorRestante; }
            set
            {
                valorRestante = value;
                this.OnPropertyChanged("ValorRestante");
            }
        }

        decimal? valorTotalReajuste = 0;
        public decimal? ValorTotalReajuste
        {
            get { return valorTotalReajuste; }
            set
            {
                valorTotalReajuste = value;
                this.OnPropertyChanged("ValorTotalReajuste");
            }
        }

        decimal? valorReajustePositivo = 0;
        public decimal? ValorReajustePositivo
        {
            get { return valorReajustePositivo; }
            set
            {
                valorReajustePositivo = value;
                this.OnPropertyChanged("ValorReajustePositivo");
            }
        }

        decimal? valorReajusteNegativo = 0;
        public decimal? ValorReajusteNegativo
        {
            get { return valorReajusteNegativo; }
            set
            {
                valorReajusteNegativo = value;
                this.OnPropertyChanged("ValorReajusteNegativo");
            }
        }

        RadButton btnEditarParcelaRegistro;
        RadButton btnExcluirParcelaRegistro;

        EnumReajuste Enum = EnumReajuste.Nenhum;

        #endregion

        #region [ Construtor ]

        public PagamentoVendaUI()
        {
            this.ListaVenda = new List<VENDA>();
            this.ListaVendaAux = new List<VENDA>();
            this.LinhasGridView = 20;
            this.ListaPagamentoVenda = new List<PAGAMENTOVENDA>();
            this.ListaPagamentoVendaRegistro = new List<PAGAMENTOVENDASREGISTRO>();
            this.PagamentoVenda = new PAGAMENTOVENDA();
            this.PagamentoVendaRegistro = new PAGAMENTOVENDASREGISTRO();

            this.radTabControlTemplate = new RadTabControl();
            this.btnSalvarParcela = new RadButton();

            InitializeComponent();

            this.Loaded += PagamentoVendaUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void PagamentoVendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaVenda = VendaDAL.RecuperarListaVendas().Where(a => a.cd_TipoPedido == 2).ToList();

                foreach (VENDA item in ListaVenda)
                    ListaVendaAux.Add(item);

            }).ContinueWith(task =>
            {
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                CarregarListaVenda(false);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());

            this.LinhasGridView = (int)((rGridView.ActualHeight - 120) / dtVenda.RowHeight);
        }

        #region [RadGridView]

        private void dtVenda_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            try
            {
                this.txtValorPago.Clear();
                this.txtValorRestante.Clear();

                if (dtVenda.SelectedItem != null)
                {
                    this.Venda = dtVenda.SelectedItem as VENDA;
                    this.CarregarGridParcelas();
                    this.CarregarCamposValoresVenda();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtParcela_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            try
            {
                if (dtParcela.SelectedItem != null)
                {
                    this.PagamentoVenda = VendaDAL.RecuperarPagamentoVenda((dtParcela.SelectedItem as PAGAMENTOVENDA).cd_PagamentoVenda);

                    this.valorRestante = PagamentoVenda.ds_ValorRestante;
                    this.txtValorReajuste.Value = null;

                    switch (enumTipoOperacao)
                    {
                        case EnumTipoOperacao.Novo: // Novo Pagamento

                            this.txtValorRestante.Text = Convert.ToDecimal((PagamentoVenda.ds_ValorReajustado - PagamentoVenda.ds_ValorRecebido)).ToString();
                            this.txtValorReajustado.Text = VendaDAL.ResgatarValorFuncCalcularReajuste(PagamentoVenda.cd_PagamentoVenda).ToString(); //Função Banco
                            this.txtValorReajusteCalculado.Text = PagamentoVenda.ds_ValorReajuste.ToString();

                            if (PagamentoVenda.cd_Reajuste == 1 || PagamentoVenda.cd_Reajuste == 3)
                                this.rdFixo.IsChecked = true;

                            else if (PagamentoVenda.cd_Reajuste == 2 || PagamentoVenda.cd_Reajuste == 4)
                                this.rdPorcentagem.IsChecked = true;

                            break;

                        case EnumTipoOperacao.Editar:

                            this.txtValorReajustado.Text = VendaDAL.ResgatarValorFuncCalcularReajuste(PagamentoVenda.cd_PagamentoVenda).ToString();
                            this.txtValorReajusteCalculado.Text = PagamentoVenda.ds_ValorReajuste.ToString();
                            this.txtValorPago.Text = Convert.ToDecimal(PagamentoVenda.ds_ValorRecebido).ToString();
                            this.txtValorRestante.Text = Convert.ToDecimal(PagamentoVenda.ds_ValorRestante).ToString();

                            if (PagamentoVenda.cd_Reajuste == 1 || PagamentoVenda.cd_Reajuste == 3)
                                this.rdFixo.IsChecked = true;

                            else if (PagamentoVenda.cd_Reajuste == 2 || PagamentoVenda.cd_Reajuste == 4)
                                this.rdPorcentagem.IsChecked = true;

                            this.btnAcrescimo.IsEnabled = true;
                            this.btnDesconto.IsEnabled = true;
                            this.txtValorReajuste.IsEnabled = true;
                            this.rdFixo.IsEnabled = true;
                            this.rdPorcentagem.IsEnabled = true;

                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [TextBox]

        private void txtValorPago_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (PagamentoVenda.ds_ValorRecebido == null)
                {
                    if (txtValorPago.Text.Length == 0)
                        this.txtValorRestante.Text = Convert.ToDecimal(PagamentoVenda.ds_ValorParcela).ToString();
                    else
                        this.txtValorRestante.Text = (Convert.ToDecimal(PagamentoVenda.ds_ValorParcela) - Convert.ToDecimal(txtValorPago.Text)).ToString();
                }

                else if (txtValorPago.Text.Length == 0)
                    this.txtValorRestante.Text = Convert.ToDecimal(PagamentoVenda.ds_ValorRestante).ToString();

                else if (EnumTipoOperacao.Novo == enumTipoOperacao)
                    this.txtValorRestante.Text = (Convert.ToDecimal(PagamentoVenda.ds_ValorRestante) - Convert.ToDecimal(txtValorPago.Text)).ToString();

                else if (pagamentoVenda.ds_ValorRecebido == null || EnumTipoOperacao.Editar == enumTipoOperacao)
                    this.txtValorRestante.Text = (Convert.ToDecimal(txtValorReajustado.Text) - Convert.ToDecimal(txtValorPago.Text)).ToString();

                else
                    this.txtValorRestante.Text = txtValorReajustado.Text == string.Empty ? string.Empty : (Convert.ToDecimal(txtValorReajustado.Text) - Convert.ToDecimal(txtValorPago.Text)).ToString();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtValorPago_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.OemComma)
                e.Handled = false;
            else
                e.Handled = true;
        }

        #endregion

        #region [RadioCombobox]

        private void rdbNaoQuitado_Checked(object sender, RoutedEventArgs e)
        {
            CarregarListaVenda(false);
        }

        private void rdbQuitado_Checked(object sender, RoutedEventArgs e)
        {
            CarregarListaVenda(true);
        }

        #endregion

        #region [Botoes]

        private void btnSalvarPagamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    if (Convert.ToDecimal(txtValorRestante.Text) < 0)
                    {
                        MessageBox.Show(Mensagens.pagamentoMaiorQueValorRestante, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    if (EnumTipoOperacao.Novo == enumTipoOperacao)
                        PagamentoVenda.ds_ValorRecebido = PagamentoVenda.ds_ValorRecebido == null ? (Convert.ToDecimal(txtValorPago.Text)) : (PagamentoVenda.ds_ValorRecebido + Convert.ToDecimal(txtValorPago.Text));

                    else
                        PagamentoVenda.ds_ValorRecebido = (Convert.ToDecimal(txtValorPago.Text));

                    PagamentoVenda.ds_ValorRestante = Convert.ToDecimal(txtValorRestante.Text);
                    PagamentoVenda.dt_Pagamento_Efetuado = DateTime.Now;
                    PagamentoVenda.cd_Reajuste = VerificarTipoReajuste();

                    PagamentoVenda.ds_ValorReajustado = Convert.ToDecimal(txtValorReajustado.Text);

                    if (txtValorReajusteCalculado.Text.Length != 0) // Caso Teve um reajuste entra no if e pega o valor do TextBox;
                        PagamentoVenda.ds_ValorReajuste = Convert.ToDecimal(txtValorReajusteCalculado.Text);

                    PAGAMENTOVENDASREGISTRO pagamentoVendasRegistro = new PAGAMENTOVENDASREGISTRO();
                    pagamentoVendasRegistro.cd_PagamentoVenda = PagamentoVenda.cd_PagamentoVenda;
                    pagamentoVendasRegistro.ds_ValorPagamento = (Convert.ToDecimal(txtValorPago.Text));
                    pagamentoVendasRegistro.dt_Pagamento = DateTime.Now;

                    VendaDAL.AlterarPagamentoVenda(PagamentoVenda, pagamentoVendasRegistro, Gerenciador.UsuarioAtivo.cd_Funcionario.Value);

                    if (MessageBox.Show(Mensagens.registroSalvoComSucessoImprimir, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        new HostPagamentoVenda(VendaDAL.RecuperarUltimoPagamentovendaRegistro()).ShowDialog();

                    txtValorPago.Clear();
                    txtValorRestante.Clear();

                    VerificarOperacao(EnumTipoOperacao.Aguardar);

                    CarregarGridParcelas();
                    CarregarCamposValoresVenda();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Novo);
        }

        private void btnEditarParcela_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Editar);
        }

        private void btnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Cancelar);
        }

        private void btnAcrescimo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PagamentoVenda.ds_ValorRestante = valorRestante; // Gambiarra feita para corrigir quando é dado um desconto e modifica junto o valor restante

                if (rdFixo.IsChecked == true)
                {
                    if (ValidarCamposReajuste())
                        txtValorReajustado.Text = VerificarDescontoAcrescimo(Convert.ToDecimal(txtValorParcela.Value), Convert.ToDecimal(txtValorReajuste.Value), EnumReajuste.Fixo_Acrescimo).ToString();
                    Enum = EnumReajuste.Fixo_Acrescimo;
                }

                else if (rdPorcentagem.IsChecked == true)
                {
                    if (ValidarCamposReajuste())
                        txtValorReajustado.Text = VerificarDescontoAcrescimo(Convert.ToDecimal(txtValorParcela.Value), Convert.ToDecimal(txtValorReajuste.Value), EnumReajuste.Porcentagem_Acrescimo).ToString();
                    Enum = EnumReajuste.Porcentagem_Acrescimo;
                }

                else
                    MessageBox.Show("Selecione o Tipo de Reajuste", Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                if (PagamentoVenda.ds_ValorReajustado == PagamentoVenda.ds_ValorParcela)
                    PagamentoVenda.ds_ValorRestante = (Convert.ToDecimal(txtValorReajusteCalculado.Text) + PagamentoVenda.ds_ValorRestante);

                else
                {
                    if ((PagamentoVenda.ds_ValorRecebido - (Convert.ToDecimal(txtValorReajusteCalculado.Text) + PagamentoVenda.ds_ValorRestante)) < 0)
                        PagamentoVenda.ds_ValorRestante = (Convert.ToDecimal(txtValorReajusteCalculado.Text) + PagamentoVenda.ds_ValorRestante) - PagamentoVenda.ds_ValorRecebido;

                    else
                        PagamentoVenda.ds_ValorRestante = (PagamentoVenda.ds_ValorRecebido - (Convert.ToDecimal(txtValorReajusteCalculado.Text) + PagamentoVenda.ds_ValorRestante));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnDesconto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdFixo.IsChecked == true)
                {
                    if (ValidarCamposReajuste())
                        txtValorReajustado.Text = VerificarDescontoAcrescimo(Convert.ToDecimal(txtValorParcela.Value), Convert.ToDecimal(txtValorReajuste.Value), EnumReajuste.Fixo_Desconto).ToString();
                    Enum = EnumReajuste.Fixo_Desconto;
                }

                else if (rdPorcentagem.IsChecked == true)
                {
                    if (ValidarCamposReajuste())
                        txtValorReajustado.Text = VerificarDescontoAcrescimo(Convert.ToDecimal(txtValorParcela.Value), Convert.ToDecimal(txtValorReajuste.Value), EnumReajuste.Porcentagem_Desconto).ToString();
                    Enum = EnumReajuste.Porcentagem_Desconto;
                }

                PagamentoVenda.ds_ValorRestante = (Convert.ToDecimal(txtValorReajusteCalculado.Text) - PagamentoVenda.ds_ValorRestante) - PagamentoVenda.ds_ValorRecebido;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #endregion

        #endregion

        #region [ Metodos ]

        public void CarregarListaVenda(bool quitado)
        {
            try
            {
                this.ListaVenda.Clear();

                if (quitado)
                    this.ListaVenda = this.ListaVendaAux.Where(a => a.cd_TipoPedido == 2 && a.statusPagamentoVenda == true).ToList();
                else
                    this.ListaVenda = this.ListaVendaAux.Where(a => a.cd_TipoPedido == 2 && a.statusPagamentoVenda == false || a.statusPagamentoVenda == null).ToList();

                if (dtVenda != null)
                    dtVenda.Rebind();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool ValidarCampos()
        {
            bool formularioValidado = true;


            if (string.IsNullOrEmpty(txtDataVenda.Text))
            {
                this.txtDataVenda.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtDataVenda.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtDataVenda.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtValorParcela.Value.ToString()))
            {
                this.txtValorParcela.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorParcela.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorParcela.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtValorPago.Text))
            {
                this.txtValorPago.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorPago.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorPago.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtValorRestante.Text))
            {
                this.txtValorRestante.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorRestante.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorRestante.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtValorRestanteVenda.Value.ToString()))
            {
                this.txtValorRestanteVenda.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorRestanteVenda.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorRestanteVenda.LimpaErrosValidacao();

            return formularioValidado;
        }

        public bool ValidarCamposReajuste()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(txtValorReajuste.Value.ToString()))
            {
                this.txtValorReajuste.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorReajuste.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorReajuste.LimpaErrosValidacao();

            return formularioValidado;
        }

        public void CarregarGridParcelas()
        {
            try
            {
                this.ListaPagamentoVenda.Clear();
                this.ListaPagamentoVenda = VendaDAL.RecuperarParcelasVenda(Venda.cd_Venda);
                this.dtParcela.Rebind();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CarregarCamposValoresVenda()
        {
            try
            {
                this.txtCliente.Text = Venda.PESSOA.nm_Fantasia;
                this.txtContato.Text = Venda.PESSOA.nm_Pessoa;
                this.txtDataVenda.Text = Venda.dt_Data.ToString();

                this.ValorParcela = 0;
                this.ValorTotal = 0;
                this.ValorTotalComReajuste = 0;
                this.ValorTotalReajuste = 0;
                this.ValorPago = 0;
                this.ValorRestante = 0;
                this.ValorReajustePositivo = 0;
                this.ValorReajusteNegativo = 0;

                foreach (var item in ListaPagamentoVenda)
                {
                    ValorTotal = item.ds_ValorParcela + ValorTotal;

                    if (item.ds_ValorReajustado == null)
                        this.ValorTotalComReajuste = this.ValorTotalComReajuste + item.ds_ValorParcela;

                    else
                        this.ValorTotalComReajuste = (this.ValorTotalComReajuste + item.ds_ValorReajustado);

                    if (item.cd_Reajuste == (int)EnumReajuste.Fixo_Acrescimo || item.cd_Reajuste == (int)EnumReajuste.Porcentagem_Acrescimo)
                        this.ValorReajustePositivo = item.ds_ValorReajuste + this.ValorReajustePositivo;

                    else if (item.cd_Reajuste == (int)EnumReajuste.Fixo_Desconto || item.cd_Reajuste == (int)EnumReajuste.Porcentagem_Desconto)
                        this.ValorReajusteNegativo = item.ds_ValorReajuste + this.ValorReajusteNegativo;

                    if (item.ds_ValorRecebido != null)
                        this.ValorPago = this.ValorPago + item.ds_ValorRecebido;

                    this.ValorParcela = item.ds_ValorParcela;
                }

                this.ValorTotalReajuste = (this.ValorReajustePositivo - this.ValorReajusteNegativo);
                this.ValorRestante = (this.ValorTotalComReajuste - this.ValorPago);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal VerificarDescontoAcrescimo(decimal valorParcela, decimal valorReajuste, EnumReajuste enumReajuste)
        {
            if (enumReajuste == EnumReajuste.Fixo_Acrescimo)
            {
                txtValorReajusteCalculado.Text = valorReajuste.ToString();
                return (valorParcela + valorReajuste);
            }

            else if (enumReajuste == EnumReajuste.Fixo_Desconto)
            {
                txtValorReajusteCalculado.Text = valorReajuste.ToString();
                return (valorParcela - valorReajuste);
            }

            else if (enumReajuste == EnumReajuste.Porcentagem_Acrescimo)
            {
                txtValorReajusteCalculado.Text = (valorParcela * (valorReajuste / 100)).ToString();
                return (valorParcela + (valorParcela * (valorReajuste / 100)));
            }

            else if (enumReajuste == EnumReajuste.Porcentagem_Desconto)
            {
                txtValorReajusteCalculado.Text = (valorParcela * (valorReajuste / 100)).ToString();
                return (valorParcela - (valorParcela * (valorReajuste / 100)));
            }

            else
                return 0;
        }

        public int VerificarTipoReajuste()
        {
            if (Enum == EnumReajuste.Fixo_Acrescimo)
                return 1;

            else if (Enum == EnumReajuste.Fixo_Desconto)
                return 3;

            else if (Enum == EnumReajuste.Porcentagem_Acrescimo)
                return 2;

            else if (Enum == EnumReajuste.Porcentagem_Desconto)
                return 4;

            else if (pagamentoVenda.cd_Reajuste == null)
                return 5;

            else if (EnumTipoOperacao.Editar == enumTipoOperacao || EnumTipoOperacao.Novo == enumTipoOperacao)
                return Convert.ToInt16(pagamentoVenda.cd_Reajuste);

            else
                return 5;
        }

        public void VerificarOperacao(EnumTipoOperacao EnumTipo)
        {
            if (EnumTipo == EnumTipoOperacao.Novo)
            {
                this.btnEditarParcela.IsEnabled = false;
                this.btnCancelarVenda.IsEnabled = true;
                this.btnSalvarPagamento.IsEnabled = true;
                this.txtValorPago.IsReadOnly = false;
                this.dtParcela.IsEnabled = true;
                this.rdPorcentagem.IsEnabled = false;
                this.rdFixo.IsEnabled = false;
                this.txtValorReajuste.IsEnabled = false;
                this.btnAcrescimo.IsEnabled = false;
                this.btnDesconto.IsEnabled = false;

                this.enumTipoOperacao = EnumTipo;

                this.PagamentoVenda = new PAGAMENTOVENDA();

                this.LimparErrosValidacao();
            }

            else if (EnumTipo == EnumTipoOperacao.Editar)
            {
                this.txtValorPago.Text = PagamentoVenda.ds_ValorRecebido != null ? Convert.ToDecimal(PagamentoVenda.ds_ValorRecebido).ToString() : "0";
                this.txtValorRestante.Text = PagamentoVenda.ds_ValorRestante != null ? Convert.ToDecimal(PagamentoVenda.ds_ValorRestante).ToString() : string.Empty;
                this.txtValorReajuste.Value = PagamentoVenda.ds_ValorReajuste != null ? Convert.ToDouble(PagamentoVenda.ds_ValorReajuste) : 0;

                this.btnNovo.IsEnabled = false;
                this.btnSalvarPagamento.IsEnabled = true;
                this.btnCancelarVenda.IsEnabled = true;
                this.txtValorPago.IsReadOnly = true;
                this.rdPorcentagem.IsEnabled = true;
                this.rdFixo.IsEnabled = true;
                this.txtValorReajuste.IsEnabled = true;

                this.dtParcela.IsEnabled = true;
                this.btnAcrescimo.IsEnabled = true;
                this.btnDesconto.IsEnabled = true;

                this.enumTipoOperacao = EnumTipo;

                this.PagamentoVenda = new PAGAMENTOVENDA();

                this.LimparErrosValidacao();
            }

            else if (EnumTipo == EnumTipoOperacao.Cancelar)
            {
                this.btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                this.btnEditarParcela.IsEnabled = Gerenciador.PermiteEditar;

                this.btnSalvarPagamento.IsEnabled = false;
                this.btnCancelarVenda.IsEnabled = false;
                this.txtValorPago.IsReadOnly = true;
                this.rdPorcentagem.IsEnabled = false;
                this.rdFixo.IsEnabled = false;
                this.txtValorReajuste.IsEnabled = false;

                this.dtParcela.IsEnabled = false;
                this.txtValorPago.Clear();
                this.txtValorReajustado.Clear();
                this.txtValorReajusteCalculado.Clear();
                this.txtValorRestante.Clear();
                this.txtValorReajuste.Value = null;
                this.btnAcrescimo.IsEnabled = false;
                this.btnDesconto.IsEnabled = false;

                this.enumTipoOperacao = EnumTipo;

                this.LimparErrosValidacao();

                this.PagamentoVenda = new PAGAMENTOVENDA();

                this.dtVenda_SelectionChanged(null, null);
            }

            else if (EnumTipo == EnumTipoOperacao.Aguardar)
            {
                this.btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                this.btnEditarParcela.IsEnabled = Gerenciador.PermiteEditar;

                this.btnSalvarPagamento.IsEnabled = true;

                this.dtParcela.IsEnabled = false;

                this.rdFixo.IsChecked = false;
                this.rdPorcentagem.IsChecked = false;
                this.rdPorcentagem.IsEnabled = false;
                this.rdFixo.IsEnabled = false;
                this.txtValorReajuste.IsEnabled = false;

                this.txtValorPago.IsReadOnly = true;

                this.txtValorPago.Clear();
                this.txtValorReajustado.Clear();
                this.txtValorReajusteCalculado.Clear();
                this.txtValorRestante.Clear();
                this.txtValorReajuste.Value = null;
                this.btnAcrescimo.IsEnabled = false;
                this.btnDesconto.IsEnabled = false;

                this.enumTipoOperacao = EnumTipo;

                this.LimparErrosValidacao();

                this.PagamentoVenda = new PAGAMENTOVENDA();

                this.dtVenda_SelectionChanged(null, null);
            }
        }

        private void LimparErrosValidacao()
        {
            this.txtDataVenda.LimpaErrosValidacao();
            this.txtValorParcela.LimpaErrosValidacao();
            this.txtValorPago.LimpaErrosValidacao();
            this.txtValorRestante.LimpaErrosValidacao();
            this.txtValorRestanteVenda.LimpaErrosValidacao();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region [Componentes do pagamento da parcela da parcela]

        #region [Eventos]

        private void btnEditardtParcelaEdicao_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = (sender as RadButton);
            parent.Visibility = enumTipoOperacao == EnumTipoOperacao.Novo ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void btnExcluirdtParcelaEdicao_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = (sender as RadButton);
            parent.Visibility = enumTipoOperacao == EnumTipoOperacao.Novo ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void btnEditardtParcelaEdicao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = (sender as RadButton).ParentOfType<GridViewRow>();

                this.PagamentoVendaRegistro = parent.Item as PAGAMENTOVENDASREGISTRO;
                parent.IsSelected = true;

                (this.radTabControlTemplate.Items[1] as RadTabItem).IsEnabled = true;
                this.radTabControlTemplate.SelectedIndex = 1;

                this.txtValorPagoParcela.Text = this.PagamentoVendaRegistro.ds_ValorPagamento.ToString();
                this.txtValorRestanteParcela.Text = this.PagamentoVenda.ds_ValorRestante.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExcluirdtParcelaEdicao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var parent = (sender as RadButton).ParentOfType<GridViewRow>();
                    this.PagamentoVendaRegistro = parent.Item as PAGAMENTOVENDASREGISTRO;
                    parent.IsSelected = true;

                    this.PagamentoVenda.ds_ValorRecebido = PagamentoVenda.ds_ValorRecebido - PagamentoVendaRegistro.ds_ValorPagamento;
                    this.PagamentoVenda.ds_ValorRestante = PagamentoVenda.ds_ValorRestante + PagamentoVendaRegistro.ds_ValorPagamento;

                    VendaDAL.ExcluirPagamentoVendaRegistro(PagamentoVendaRegistro);
                    VendaDAL.AlterarPagamentoVenda(PagamentoVenda);

                    this.dtParcela_SelectionChanged(null, null);

                    this.CarregarGridParcelas();
                    this.CarregarCamposValoresVenda();

                    this.VerificarOperacao(EnumTipoOperacao.Aguardar);
                    new MensagemUI(Mensagens.registroExcluidoComSucesso);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimirdtParcelaEdicao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = (sender as RadButton).ParentOfType<GridViewRow>();

                PagamentoVendaRegistro = parent.Item as PAGAMENTOVENDASREGISTRO;
                parent.IsSelected = true;

                new HostPagamentoVenda(PagamentoVendaRegistro.cd_PagamentoVendaRegistros).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtParcela_LoadingRowDetails(object sender, GridViewRowDetailsEventArgs e)
        {
            try
            {
                this.radTabControlTemplate = e.DetailsElement.FindName("rtParcelas") as RadTabControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtValorRestantedtParcelaEdicao_Loaded(object sender, RoutedEventArgs e)
        {
            txtValorRestanteParcela = (TextBox)sender;
        }

        private void txtValorPagodtParcelaEdicao_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtValorPagoParcela = (TextBox)sender;
                this.txtValorPagoParcela.KeyUp += txtValorPago_KeyDown;
                this.txtValorPagoParcela.TextChanged += txtValorPagoParcela_TextChanged;

                if (this.enumTipoOperacao == EnumTipoOperacao.Novo)
                    txtValorPagoParcela.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtValorPagoParcela_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                decimal valorRestanteParcela = (decimal)PagamentoVenda.ds_ValorRestante + (decimal)PagamentoVendaRegistro.ds_ValorPagamento;

                if (txtValorRestanteParcela != null)
                {
                    if (txtValorPagoParcela.Text.Length != 0)
                        txtValorRestanteParcela.Text = (valorRestanteParcela - Convert.ToDecimal(txtValorPagoParcela.Text)).ToString();

                    else
                        txtValorRestanteParcela.Text = valorRestanteParcela.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSalvarParcela_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCamposEdicaoParcela())
                {
                    decimal novoValorPago = Convert.ToDecimal(this.txtValorPagoParcela.Text);
                    decimal diferencaValor = novoValorPago - (decimal)this.PagamentoVendaRegistro.ds_ValorPagamento;

                    this.PagamentoVenda.ds_ValorRecebido = this.PagamentoVenda.ds_ValorRecebido + diferencaValor;
                    this.PagamentoVenda.ds_ValorRestante = this.PagamentoVenda.ds_ValorRestante - diferencaValor;

                    this.PagamentoVendaRegistro.ds_ValorPagamento = novoValorPago;

                    VendaDAL.AlterarPagamentoVenda(PagamentoVenda, PagamentoVendaRegistro, Gerenciador.UsuarioAtivo.cd_Funcionario.Value);

                    if (MessageBox.Show(Mensagens.registroSalvoComSucessoImprimir, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        new HostPagamentoVenda(PagamentoVendaRegistro.cd_PagamentoVendaRegistros).ShowDialog();

                    new MensagemUI(Mensagens.registroSalvoComSucesso);

                    this.LimparCamposEdicaoParcela();

                    this.VerificarOperacao(EnumTipoOperacao.Aguardar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelarParcela_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                btnCancelarParcela = (RadButton)sender;

                if (this.enumTipoOperacao == EnumTipoOperacao.Editar)
                    btnCancelarParcela.Click += btnCancelarParcela_Click;

                else
                    btnCancelarParcela.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelarParcela_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LimparCamposEdicaoParcela();
                this.VerificarOperacao(EnumTipoOperacao.Aguardar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtParcela_RowDetailsVisibilityChanging(object sender, RowDetailsVisibilityChangingEventArgs e)
        {
            try
            {
                if (e.Row != null)
                    e.Row.IsSelected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [Metodos]

        private bool ValidarCamposEdicaoParcela()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(txtValorPagoParcela.Text))
            {
                this.txtValorPagoParcela.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorPagoParcela.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorPagoParcela.LimpaErrosValidacao();

            if (Convert.ToDecimal(txtValorRestanteParcela.Text) < 0)
            {
                this.txtValorRestanteParcela.SetaValidacao("CAMPO_OBRIGATORIO");
                this.txtValorRestanteParcela.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.txtValorRestanteParcela.LimpaErrosValidacao();


            return formularioValidado;
        }

        private void LimparCamposEdicaoParcela()
        {
            this.txtValorPagoParcela.Clear();
            this.txtValorRestanteParcela.Clear();
            this.radTabControlTemplate.SelectedIndex = 0;
        }

        #endregion

        #endregion
    }
}