using System;
using System.Collections.Generic;
using System.Windows;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace NaMidia
{
    public partial class FormaPagamentoUI : RadWindow, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

        private decimal valorVenda;
        public decimal ValorVenda
        {
            get { return valorVenda; }
            set
            {
                valorVenda = value;
                OnPropertyChanged("ValorVenda");
            }
        }

        private List<FORMAPAGAMENTO> listaFormaPagamento;
        public List<FORMAPAGAMENTO> ListaFormaPagamento
        {
            get { return listaFormaPagamento; }
            set
            {
                listaFormaPagamento = value;
                OnPropertyChanged("ListaFormaPagamento");
            }
        }


        private List<PAGAMENTOVENDA> listaPagamentoVenda;
        public List<PAGAMENTOVENDA> ListaPagamentoVenda
        {
            get { return listaPagamentoVenda; }
            set
            {
                listaPagamentoVenda = value;
                OnPropertyChanged("ListaPagamentoVenda");
            }
        }

        private int cd_FormaPagamento;
        public int Cd_FormaPagamento
        {
            get { return cd_FormaPagamento; }
            set
            {
                cd_FormaPagamento = value;
                OnPropertyChanged("Cd_FormaPagamento");
            }
        }

        #endregion

        #region [ Construtor ]

        public FormaPagamentoUI(decimal valorVenda)
        {
            this.ValorVenda = valorVenda;
            this.ListaFormaPagamento = FormaPagamentoDAL.RecuperarListaFormaPagamento();
            this.ListaPagamentoVenda = new List<PAGAMENTOVENDA>();

            InitializeComponent();
            Loaded += FormaPagamentoUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void FormaPagamentoUI_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }
        }

        private void cbFormaPagamento_Loaded(object sender, RoutedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            combo.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
        }

        private void btnCarregarLista_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    this.CarregarParcelas((decimal)ValorVenda, Convert.ToInt32(cbParcela.Text), (DateTime)txtData.SelectedDate);
                    this.dtParcelas.Rebind();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSalvarVenda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ListaFormaPagamento.Count == 0)
                    throw new Exception(Mensagens.faltamInformacoes);

                else if (ValidarCampos())
                    this.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbFormaPagamento_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RadComboBox combo = (RadComboBox)sender;
            this.Cd_FormaPagamento = Convert.ToInt32(combo.SelectedValue);
        }

        #endregion

        #region [ Metodos ]

        public bool ValidarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(txtValorVenda.Text))
            {
                txtValorVenda.SetaValidacao("CAMPO_OBRIGATORIO");
                txtValorVenda.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtValorVenda.LimpaErrosValidacao();

            if (Convert.ToInt32(cbFormaPagamento.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbFormaPagamento.SetaValidacao("CAMPO_OBRIGATORIO");
                cbFormaPagamento.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbParcela.Text))
            {
                cbFormaPagamento.SetaValidacao("CAMPO_OBRIGATORIO");
                cbFormaPagamento.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbFormaPagamento.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtData.Text))
            {
                txtData.SetaValidacao("CAMPO_OBRIGATORIO");
                txtData.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtData.LimpaErrosValidacao();

            return formularioValidado;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public void CarregarParcelas(decimal valorVenda, int qntdParcelas, DateTime primeiroPagamento)
        {
            DateTime dataAux = primeiroPagamento;
            this.ListaPagamentoVenda.Clear();

            for (int i = 1; i <= qntdParcelas; i++)
            {
                PAGAMENTOVENDA pgmVenda = new PAGAMENTOVENDA();

                if (Cd_FormaPagamento == (int)EnumFormaPagamento.Cartão_de_Crédito)
                {
                    pgmVenda.ds_ValorParcela = (valorVenda / qntdParcelas);
                    pgmVenda.dt_Pagamento_Prevista = dataAux == primeiroPagamento ? primeiroPagamento : dataAux;
                    pgmVenda.ds_ValorReajustado = pgmVenda.ds_ValorParcela;
                    pgmVenda.ds_ValorRestante = 0;
                    pgmVenda.ds_ValorReajuste = 0;
                    pgmVenda.statusPagamentoParcela = true;
                    pgmVenda.cd_Reajuste = (int)EnumReajuste.Nenhum;
                    pgmVenda.ds_ValorRecebido = pgmVenda.ds_ValorParcela;
                    pgmVenda.exibirNotificacao = false;
                }

                else if (Cd_FormaPagamento == (int)EnumFormaPagamento.Parcelado_Loja)
                {
                    pgmVenda.ds_ValorParcela = (valorVenda / qntdParcelas);
                    pgmVenda.dt_Pagamento_Prevista = dataAux == primeiroPagamento ? primeiroPagamento : dataAux;
                    pgmVenda.ds_ValorReajustado = pgmVenda.ds_ValorParcela;
                    pgmVenda.ds_ValorRestante = pgmVenda.ds_ValorParcela;
                    pgmVenda.ds_ValorReajuste = 0;
                    pgmVenda.statusPagamentoParcela = false;
                    pgmVenda.cd_Reajuste = (int)EnumReajuste.Nenhum;
                    pgmVenda.ds_ValorRecebido = 0;
                    pgmVenda.exibirNotificacao = true;
                }

                this.ListaPagamentoVenda.Add(pgmVenda);
                dataAux = dataAux.AddDays(30);
            }
        }

        #endregion
    }
}
