using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace NaMidia.UI
{
    public partial class CostureiraPedidoRelacaoUI : RadWindow, INotifyPropertyChanged
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
                OnPropertyChanged("listaCostureiraPedido");
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

        private ITENSVENDA itensVenda;
        public ITENSVENDA ItensVenda
        {
            get { return itensVenda; }
            set
            {
                itensVenda = value;
                OnPropertyChanged("ItensVenda");
            }
        }

        private COSTUREIRAPEDIDO costureiraPedidoSelecionado;
        public COSTUREIRAPEDIDO CostureiraPedidoSelecionado
        {
            get { return costureiraPedidoSelecionado; }
            set
            {
                costureiraPedidoSelecionado = value;
                OnPropertyChanged("CostureiraPedidoSelecionado");
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

        int cd_Venda;
        public int Cd_Venda
        {
            get { return cd_Venda; }
            set
            {
                cd_Venda = value;
                OnPropertyChanged("Cd_Venda");
            }
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        #endregion

        #region [Construtor]

        public CostureiraPedidoRelacaoUI(int vendaId)
        {
            try
            {
                this.ListaCostureiraPedido = new List<COSTUREIRAPEDIDO>();
                this.ListaItensVenda = VendaDAL.RecuperarListaItensVenda(vendaId).OrderBy(a => a.PRODUTO.ds_OrdemExibicao).ThenBy(a => a.TAMANHO.ds_OrdemExibicao).ToList();

                this.ItensVenda = new ITENSVENDA();

                this.Cd_Venda = vendaId;

                InitializeComponent();
                Loaded += CostureiraPedidoRelacaoUI_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public CostureiraPedidoRelacaoUI()
        {
            InitializeComponent();
            Loaded += CostureiraPedidoRelacaoUI_Loaded;
        }

        #endregion

        #region [Eventos]

        private void CostureiraPedidoRelacaoUI_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void RadMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadMenuItem item = (RadMenuItem)sender;
                CostureiraPedidoSelecionado = ListaCostureiraPedido.FirstOrDefault(a => a.cd_CostureiraPedido == (item.ParentOfType<RadContextMenu>().GetClickedElement<GridViewRow>().Item as COSTUREIRAPEDIDO).cd_CostureiraPedido);

                if (costureiraPedidoSelecionado == null)
                    return;

                if (item.Header.ToString() == "Excluir")
                {
                    CostureiraPedidoDAL.ExcluirCostureiraPedido(CostureiraPedidoSelecionado.cd_CostureiraPedido);

                    ListaCostureiraPedido = CostureiraPedidoDAL.RecuperarListaCostureiraPedido(ItensVenda.cd_ItensVenda);
                    dtCostureiraPedido.Rebind();

                    this.txtQuantidadeDisponivel.Text = CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(CostureiraPedidoSelecionado.ITENSVENDA).ToString();
                    new MensagemUI(Mensagens.registroExcluidoComSucesso).Show();
                }

                else if (item.Header.ToString() == "Selecionar")
                {
                    this.cbCostureira.SelectedValue = CostureiraPedidoSelecionado.cd_Costureira;
                    this.rnudQuantidade.Value = CostureiraPedidoSelecionado.ds_Quantidade;
                    this.rnudValorUnit.Value = (double)CostureiraPedidoSelecionado.ds_ValorUnit;

                    int? quantidadeTotal = CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(ItensVenda);
                    this.txtQuantidadeDisponivel.Text = (quantidadeTotal + CostureiraPedidoSelecionado.ds_Quantidade).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    COSTUREIRAPEDIDO costureiraPedido = new COSTUREIRAPEDIDO();
                    costureiraPedido.cd_ItensVenda = ItensVenda.cd_ItensVenda;
                    costureiraPedido.cd_Costureira = Convert.ToInt32(cbCostureira.SelectedValue);
                    costureiraPedido.cd_CostureiraPedido = CostureiraPedidoSelecionado == null ? 0 : CostureiraPedidoSelecionado.cd_CostureiraPedido;
                    costureiraPedido.ds_Quantidade = (int)rnudQuantidade.Value;
                    costureiraPedido.ds_ValorUnit = (decimal)rnudValorUnit.Value;
                    costureiraPedido.ds_ValorTotal = costureiraPedido.ds_Quantidade * costureiraPedido.ds_ValorUnit;
                    costureiraPedido.ds_DataPagamento = null;
                    costureiraPedido.ds_Data = DateTime.Now;
                    costureiraPedido.statusPagamento = false;
                    CostureiraPedidoDAL.AlterarCostureiraPedido(costureiraPedido);

                    ListaCostureiraPedido = CostureiraPedidoDAL.RecuperarListaCostureiraPedido(ItensVenda.cd_ItensVenda);
                    dtCostureiraPedido.Rebind();

                    this.VerificarOperacao(EnumTipoOperacao.Aguardar);
                    this.txtQuantidadeDisponivel.Text = CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(ItensVenda).ToString();

                    new MensagemUI(Mensagens.registroSalvoComSucesso).Show();

                    this.rbAguarde.IsBusy = true;
                    this.worker.RunWorkerAsync();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.VerificarOperacao(EnumTipoOperacao.Aguardar);
        }

        private void dtCostureiraPedidoProduto_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            try
            {
                RadGridView rd = (RadGridView)sender;

                ItensVenda = rd.SelectedItem as ITENSVENDA;
                this.txtQuantidadeDisponivel.Text = CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(ItensVenda).ToString();
                this.ListaCostureiraPedido = CostureiraPedidoDAL.RecuperarListaCostureiraPedido(itensVenda.cd_ItensVenda);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtCostureiraPedidoProduto_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RadGridView rd = (RadGridView)sender;
                rd.SelectedItem = rd.Items[0];
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

        public void AtualizarGrid()
        {
            CostureiraPedidoDAL.AtualizarStatusPedidoCostureira(Cd_Venda, this.VerificarSeTodosItensForamRelacionados(ListaItensVenda));
            this.ListaItensVenda = VendaDAL.RecuperarListaItensVenda(Cd_Venda).ToList();
        }

        public void VerificarOperacao(EnumTipoOperacao enumTipoOperacao)
        {
            switch (enumTipoOperacao)
            {
                case EnumTipoOperacao.Aguardar:
                    this.txtQuantidadeDisponivel.Text = CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(ItensVenda).ToString();
                    this.cbCostureira.SelectedValue = null;
                    this.rnudQuantidade.Value = 0;
                    this.rnudValorUnit.Value = 0;
                    this.CostureiraPedidoSelecionado = null;
                    break;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public bool ValidarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(rnudQuantidade.Value.ToString()))
            {
                rnudQuantidade.SetaValidacao("CAMPO_OBRIGATORIO");
                rnudQuantidade.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                rnudQuantidade.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(rnudValorUnit.Value.ToString()))
            {
                rnudValorUnit.SetaValidacao("CAMPO_OBRIGATORIO");
                rnudValorUnit.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                rnudValorUnit.LimpaErrosValidacao();


            if (cbCostureira.SelectedValue == null)
            {
                cbCostureira.SetaValidacao("CAMPO_OBRIGATORIO");
                cbCostureira.RaiseErroValidacao();
                formularioValidado = false;
            }


            else if (string.IsNullOrEmpty(cbCostureira.Text))
            {
                cbCostureira.SetaValidacao("CAMPO_OBRIGATORIO");
                cbCostureira.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbCostureira.LimpaErrosValidacao();

            if (!string.IsNullOrEmpty(rnudQuantidade.Value.ToString()))
            {
                if (rnudQuantidade.Value > Convert.ToInt32(txtQuantidadeDisponivel.Text))
                {
                    rnudQuantidade.SetaValidacao("CAMPO_OBRIGATORIO");
                    rnudQuantidade.RaiseErroValidacao();
                    MessageBox.Show(Mensagens.valorQuantidadeMaiorQueDisponivel, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                    formularioValidado = false;
                }

                if (rnudQuantidade.Value == 0)
                {
                    rnudQuantidade.SetaValidacao("CAMPO_OBRIGATORIO");
                    rnudQuantidade.RaiseErroValidacao();

                    MessageBox.Show(Mensagens.valorNaoPodeSer0, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                    formularioValidado = false;
                }
            }

            if (!string.IsNullOrEmpty(rnudValorUnit.Value.ToString()))
            {
                if (rnudValorUnit.Value == 0)
                {
                    rnudValorUnit.SetaValidacao("CAMPO_OBRIGATORIO");
                    rnudValorUnit.RaiseErroValidacao();

                    MessageBox.Show(Mensagens.valorNaoPodeSer0, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                    formularioValidado = false;
                }
            }

            return formularioValidado;
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

        #endregion

    }
}
