using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using NaMidia.RELATORIOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Threading.Tasks;
using System.Threading;

namespace NaMidia.UI
{
    public partial class VendaUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

        PESSOA pessoa;
        public PESSOA Pessoa
        {
            get { return pessoa; }
            set
            {
                pessoa = value;
                OnPropertyChanged("Pessoa");
            }
        }

        List<ITENSVENDA> listaItensVenda;
        public List<ITENSVENDA> ListaItensVenda
        {
            get { return listaItensVenda; }
            set
            {
                listaItensVenda = value;
                OnPropertyChanged("ListaItensVenda");
            }
        }

        List<ITENSVENDA> listaItensVendaAux;
        public List<ITENSVENDA> ListaItensVendaAux
        {
            get { return listaItensVendaAux; }
            set
            {
                listaItensVendaAux = value;
                OnPropertyChanged("ListaItensVendaAux");
            }
        }

        VENDA venda;
        public VENDA Venda
        {
            get { return venda; }
            set
            {
                venda = value;
                OnPropertyChanged("Venda");
            }
        }

        IMAGEMVENDA imagemVenda;
        public IMAGEMVENDA ImagemVenda
        {
            get { return imagemVenda; }
            set
            {
                imagemVenda = value;
                OnPropertyChanged("ImagemVenda");
            }
        }

        FileStream fs = null;
        byte[] DadosImagem { get; set; }

        ITENSVENDA itensVenda;
        public ITENSVENDA ItensVenda
        {
            get { return itensVenda; }
            set
            {
                itensVenda = value;
                OnPropertyChanged("ItensVenda");
            }
        }

        ITENSVENDA itensVendaRecuperado;
        public ITENSVENDA ItensVendaRecuperado
        {
            get { return itensVendaRecuperado; }
            set
            {
                itensVendaRecuperado = value;
                OnPropertyChanged("ItensVendaRecuperado");
            }
        }

        List<PRODUTO> listaProduto;
        public List<PRODUTO> ListaProduto
        {
            get { return listaProduto; }
            set
            {
                listaProduto = value;
                OnPropertyChanged("ListaProduto");
            }
        }

        List<TAMANHO> listaTamanho;
        public List<TAMANHO> ListaTamanho
        {
            get { return listaTamanho; }
            set
            {
                listaTamanho = value;
                OnPropertyChanged("ListaTamanho");
            }
        }

        List<MALHA> listaMalha;
        public List<MALHA> ListaMalha
        {
            get { return listaMalha; }
            set
            {
                listaMalha = value;
                OnPropertyChanged("ListaMalha");
            }
        }

        List<GOLA> listaGola;
        public List<GOLA> ListaGola
        {
            get { return listaGola; }
            set
            {
                listaGola = value;
                OnPropertyChanged("ListaGola");
            }
        }

        EnumTipoOperacao tipoOperacao;
        #endregion

        #region [ Construtor ]

        public VendaUI()
        {
            #region Inicializando Variaveis
            this.Venda = new VENDA();
            this.Pessoa = new PESSOA();
            this.ImagemVenda = new IMAGEMVENDA();

            this.ItensVenda = new ITENSVENDA();
            this.ListaItensVenda = new List<ITENSVENDA>();
            this.ListaItensVendaAux = new List<ITENSVENDA>();

            this.ListaGola = new List<GOLA>();
            this.ListaMalha = new List<MALHA>();
            this.ListaProduto = new List<PRODUTO>();
            this.ListaTamanho = new List<TAMANHO>();
            #endregion

            InitializeComponent();
            this.Loaded += VendaUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        #region [Botoes]

        private void VendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaGola = GolaDAL.RecuperarListaGolas();
                this.ListaMalha = MalhaDAL.RecuperarListaMalhas();
                this.ListaProduto = ProdutoDAL.RecuperarListaProdutos();
                this.ListaTamanho = TamanhoDAL.RecuperarListaTamanhos();
            }).ContinueWith(task =>
            {
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void btnProcurarImagem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                    DadosImagem = new byte[fs.Length];
                    fs.Read(DadosImagem, 0, System.Convert.ToInt32(fs.Length));
                    fs.Close();

                    ImageSourceConverter imgs = new ImageSourceConverter();
                    ImagemCamiseta.SetValue(Image.SourceProperty, imgs.ConvertFromString(dlg.FileName.ToString()));
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
                if (ValidarCampos())
                {
                    Venda.cd_TipoPedido = rbOrcamento.IsChecked == true ? 1 : 2;
                    ImagemVenda.ds_Imagem = DadosImagem == null ? ImagemVenda.ds_Imagem : DadosImagem;
                    Venda.exibirNotificacao = true;

                    if (Venda.cd_TipoPedido == (int)EnumStatusVenda.TipoVenda.Venda)
                    {
                        FormaPagamentoUI formaPag = new FormaPagamentoUI((decimal)ListaItensVenda.Sum(a => a.ds_SubTotal));
                        formaPag.ShowDialog();

                        if (formaPag.ListaPagamentoVenda.Count > 0)
                        {
                            Venda.cd_FormaPagamento = formaPag.Cd_FormaPagamento;
                            Venda.statusPagamentoVenda = Venda.cd_FormaPagamento == (int)EnumFormaPagamento.Cartão_de_Crédito ? true : false;

                            VendaDAL.AlterarVenda(Venda, ListaItensVenda, formaPag.ListaPagamentoVenda, ImagemVenda, Gerenciador.UsuarioAtivo.cd_Funcionario.Value);
                            new MensagemUI(Mensagens.registroSalvoComSucesso).Show();

                            if (MessageBox.Show(Mensagens.DesejaImprimirComprovante, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                new HostVenda(this.tipoOperacao == EnumTipoOperacao.Novo ? VendaDAL.RecuperarUltimaVenda() : Venda.cd_Venda).ShowDialog();

                            VerificarOperacao(EnumTipoOperacao.Aguardar);
                        }

                        else
                            MessageBox.Show(Mensagens.faltamInformacoesVenda, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    else if (Venda.cd_TipoPedido == (int)EnumStatusVenda.TipoVenda.Orçamento)
                    {
                        VendaDAL.AlterarVenda(Venda, ListaItensVenda, ImagemVenda, Gerenciador.UsuarioAtivo.cd_Funcionario.Value);
                        new MensagemUI(Mensagens.registroSalvoComSucesso).Show();

                        if (MessageBox.Show(Mensagens.DesejaImprimirComprovante, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            new HostVenda(this.tipoOperacao == EnumTipoOperacao.Novo ? VendaDAL.RecuperarUltimaVenda() : Venda.cd_Venda).ShowDialog();

                        VerificarOperacao(EnumTipoOperacao.Aguardar);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCamposItensVenda())
                {
                    if (this.txbAddItem.Text == "Salvar Item")
                    {
                        this.txbAddItem.Text = "Adicionar Item";
                        Uri iconUri = new Uri("pack://application:,,,/Imagens/adicionarItem_icone.png", UriKind.RelativeOrAbsolute);
                        this.imgSalvarItem.Source = BitmapFrame.Create(iconUri);
                        this.btnAddItem.Width = 110;
                    }

                    Task.Factory.StartNew(() =>
                    {
                        if (this.ItensVendaRecuperado != null)
                        {
                            ITENSVENDA item = ListaItensVendaAux.FirstOrDefault(a => a.cd_Gola == ItensVendaRecuperado.cd_Gola &&
                                                                                a.ds_Quantidade == ItensVendaRecuperado.ds_Quantidade &&
                                                                                a.cd_Produto == ItensVendaRecuperado.cd_Produto &&
                                                                                a.cd_Tamanho == ItensVendaRecuperado.cd_Tamanho &&
                                                                                a.cd_Gola == ItensVendaRecuperado.cd_Gola &&
                                                                                a.ds_ValorUnitario == ItensVendaRecuperado.ds_ValorUnitario);

                            item.ds_Quantidade = (int)rnudQuantidade.Value;
                            item.PRODUTO = ProdutoDAL.RecuperarProduto(Convert.ToInt32(cbProduto.SelectedValue));
                            item.TAMANHO = TamanhoDAL.RecuperarTamanho(Convert.ToInt32(cbTamanho.SelectedValue));
                            item.MALHA = MalhaDAL.RecuperarMalha(Convert.ToInt32(cbMalha.SelectedValue));
                            item.GOLA = GolaDAL.RecuperarGola(Convert.ToInt32(cbGola.SelectedValue));
                            item.VENDA = new VENDA();
                            item.ds_ValorUnitario = (decimal)rnudValorUnit.Value;
                            item.ds_SubTotal = (item.ds_Quantidade * item.ds_ValorUnitario);
                            item.ds_Observacoes = txtObservacoesItens.Text;
                            ItensVendaRecuperado = null;
                        }

                        else
                        {
                            ItensVenda = new ITENSVENDA();
                            ItensVenda.ds_Quantidade = (int)rnudQuantidade.Value;
                            ItensVenda.PRODUTO = ProdutoDAL.RecuperarProduto(Convert.ToInt32(cbProduto.SelectedValue));
                            ItensVenda.TAMANHO = TamanhoDAL.RecuperarTamanho(Convert.ToInt32(cbTamanho.SelectedValue));
                            ItensVenda.MALHA = MalhaDAL.RecuperarMalha(Convert.ToInt32(cbMalha.SelectedValue));
                            ItensVenda.GOLA = GolaDAL.RecuperarGola(Convert.ToInt32(cbGola.SelectedValue));
                            ItensVenda.VENDA = new VENDA();
                            ItensVenda.ds_ValorUnitario = (decimal)rnudValorUnit.Value;
                            ItensVenda.ds_SubTotal = (ItensVenda.ds_Quantidade * ItensVenda.ds_ValorUnitario);
                            ItensVenda.ds_Observacoes = txtObservacoesItens.Text;

                            ListaItensVendaAux.Add(ItensVenda);
                        }

                        this.ListaItensVenda.Clear();

                        foreach (var item in this.ListaItensVendaAux.OrderBy(a => a.PRODUTO.ds_OrdemExibicao).ThenBy(a => a.TAMANHO.ds_OrdemExibicao))
                            this.ListaItensVenda.Add(item);

                        dtItensVenda.Rebind();
                        LimparCamposItensVenda();

                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                }

                else
                    MessageBox.Show(Mensagens.faltamInformacoes, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            this.VerificarOperacao(EnumTipoOperacao.Novo);
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            this.VerificarOperacao(EnumTipoOperacao.Editar);
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            this.VerificarOperacao(EnumTipoOperacao.Excluir);
        }

        private void btnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            this.VerificarOperacao(EnumTipoOperacao.Cancelar);
        }

        private void btnPesquisarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PesquisaClienteUI pesq = new PesquisaClienteUI();
                pesq.ShowDialog();

                if (pesq.concluiu)
                {
                    Pessoa = pesq.Pessoa;
                    Venda.cd_Pessoa = pesq.Pessoa.cd_Pessoa;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [Combobox]

        private void cbProduto_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RadComboBox combo = sender as RadComboBox;
                combo.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbTamanho_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RadComboBox combo = sender as RadComboBox;
                combo.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbGola_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RadComboBox combo = sender as RadComboBox;
                combo.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbMalha_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RadComboBox combo = sender as RadComboBox;
                combo.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [RadGridView]

        private void dtItensVenda_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                this.ItensVenda = (sender as RadGridView).SelectedItem as ITENSVENDA;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void dtItensVenda_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            try
            {
                this.ItensVenda.ds_SubTotal = (ItensVenda.ds_Quantidade * ItensVenda.ds_ValorUnitario);
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
                var linhaRecuperada = item.ParentOfType<RadContextMenu>().GetClickedElement<GridViewRow>();

                if (linhaRecuperada != null)
                {
                    ItensVendaRecuperado = linhaRecuperada.Item as ITENSVENDA;

                    if (item.Header.ToString() == "Editar")
                    {
                        if (ItensVendaRecuperado != null)
                        {
                            this.rnudQuantidade.Value = ItensVendaRecuperado.ds_Quantidade;
                            this.cbProduto.SelectedValue = ItensVendaRecuperado.cd_Produto;
                            this.cbTamanho.SelectedValue = ItensVendaRecuperado.cd_Tamanho;
                            this.cbMalha.SelectedValue = ItensVendaRecuperado.cd_Malha;
                            this.cbGola.SelectedValue = ItensVendaRecuperado.cd_Gola;
                            this.rnudValorUnit.Value = (double)ItensVendaRecuperado.ds_ValorUnitario;
                            this.txtObservacoesItens.Text = ItensVendaRecuperado.ds_Observacoes;
                            this.txbAddItem.Text = "Salvar Item";
                            Uri iconUri = new Uri("pack://application:,,,/Imagens/salvarItem_icone.png", UriKind.RelativeOrAbsolute);
                            this.imgSalvarItem.Source = BitmapFrame.Create(iconUri);
                            this.btnAddItem.Width = 95;
                        }
                    }

                    else if (item.Header.ToString() == "Excluir")
                    {
                        this.ListaItensVenda.Remove(ItensVendaRecuperado);
                        this.ListaItensVendaAux.Remove(ItensVendaRecuperado);
                        this.dtItensVenda.Rebind();

                        this.LimparCamposItensVenda();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #endregion

        #region [ Metodos ]

        public bool ValidarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(txtNome.Text))
            {
                txtNome.SetaValidacao("CAMPO_OBRIGATORIO");
                txtNome.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtNome.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(Convert.ToString(DadosImagem)))
            {
                ImagemCamiseta.SetaValidacao("CAMPO_OBRIGATORIO");
                ImagemCamiseta.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                ImagemCamiseta.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(this.dpDataPrevista.Text))
            {
                this.dpDataPrevista.SetaValidacao("CAMPO_OBRIGATORIO");
                this.dpDataPrevista.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                this.dpDataPrevista.LimpaErrosValidacao();


            if (ListaItensVenda.Count == 0)
            {
                dtItensVenda.SetaValidacao("CAMPO_OBRIGATORIO");
                dtItensVenda.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                dtItensVenda.LimpaErrosValidacao();

            return formularioValidado;
        }

        public bool ValidarCamposItensVenda()
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

            if (Convert.ToInt32(cbProduto.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbProduto.SetaValidacao("CAMPO_OBRIGATORIO");
                cbProduto.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbProduto.Text))
            {
                cbProduto.SetaValidacao("CAMPO_OBRIGATORIO");
                cbProduto.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbProduto.LimpaErrosValidacao();

            if (Convert.ToInt32(cbTamanho.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbTamanho.SetaValidacao("CAMPO_OBRIGATORIO");
                cbTamanho.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbTamanho.Text))
            {
                cbTamanho.SetaValidacao("CAMPO_OBRIGATORIO");
                cbTamanho.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbTamanho.LimpaErrosValidacao();

            if (Convert.ToInt32(cbGola.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbGola.SetaValidacao("CAMPO_OBRIGATORIO");
                cbGola.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbGola.Text))
            {
                cbGola.SetaValidacao("CAMPO_OBRIGATORIO");
                cbGola.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbGola.LimpaErrosValidacao();

            if (Convert.ToInt32(cbGola.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbGola.SetaValidacao("CAMPO_OBRIGATORIO");
                cbGola.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbGola.Text))
            {
                cbGola.SetaValidacao("CAMPO_OBRIGATORIO");
                cbGola.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbGola.LimpaErrosValidacao();

            if (Convert.ToInt32(cbMalha.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbMalha.SetaValidacao("CAMPO_OBRIGATORIO");
                cbMalha.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbMalha.Text))
            {
                cbMalha.SetaValidacao("CAMPO_OBRIGATORIO");
                cbMalha.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbMalha.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(rnudValorUnit.Value.ToString()))
            {
                rnudValorUnit.SetaValidacao("CAMPO_OBRIGATORIO");
                rnudValorUnit.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                rnudValorUnit.LimpaErrosValidacao();

            return formularioValidado;
        }

        public void LimparValidacaoCampos()
        {
            ImagemCamiseta.LimpaErrosValidacao();
            this.dpDataPrevista.LimpaErrosValidacao();
            rnudQuantidade.LimpaErrosValidacao();
            cbProduto.LimpaErrosValidacao();
            cbTamanho.LimpaErrosValidacao();
            cbGola.LimpaErrosValidacao();
            cbMalha.LimpaErrosValidacao();
            rnudValorUnit.LimpaErrosValidacao();
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            try
            {
                switch (Tipo)
                {
                    case EnumTipoOperacao.Novo:
                        this.Venda = new VENDA();
                        this.Venda.statusVenda = true;
                        this.ImagemVenda = new IMAGEMVENDA();

                        this.cbGola.IsEnabled = true;
                        this.cbMalha.IsEnabled = true;
                        this.cbProduto.IsEnabled = true;
                        this.cbTamanho.IsEnabled = true;
                        this.rnudQuantidade.IsEnabled = true;
                        this.rnudValorUnit.IsEnabled = true;
                        this.txtObservacoes.IsEnabled = true;
                        this.txtObservacoesItens.IsEnabled = true;
                        this.dpDataPrevista.Text = string.Empty;
                        this.dpDataPrevista.IsEnabled = true;
                        this.txtObservacoesItens.IsEnabled = true;

                        this.cbGola.Text = string.Empty;
                        this.cbMalha.Text = string.Empty;
                        this.cbProduto.Text = string.Empty;
                        this.cbTamanho.Text = string.Empty;
                        this.rnudQuantidade.Value = null;
                        this.rnudValorUnit.Value = null;
                        this.txtObservacoesItens.Text = string.Empty;
                        this.txtObservacoes.Text = string.Empty;
                        this.txtObservacoesItens.Text = string.Empty;

                        this.txtNome.Text = string.Empty;

                        this.rnudQuantidade.Value = null;

                        this.btnNovo.IsEnabled = false;
                        this.btnEditar.IsEnabled = false;
                        this.btnExcluir.IsEnabled = false;
                        this.btnCancelarVenda.IsEnabled = true;
                        this.btnPesquisarCliente.IsEnabled = true;
                        this.btnProcurarImagem.IsEnabled = true;
                        this.btnAddItem.IsEnabled = true;
                        this.btnSalvarVenda.IsEnabled = true;

                        this.DadosImagem = null;

                        this.rbVenda.IsChecked = true;

                        this.ListaItensVenda.Clear();
                        this.ListaItensVendaAux.Clear();

                        this.tipoOperacao = EnumTipoOperacao.Novo;
                        break;

                    case EnumTipoOperacao.Editar:
                        this.cbGola.IsEnabled = true;
                        this.cbMalha.IsEnabled = true;
                        this.cbProduto.IsEnabled = true;
                        this.cbTamanho.IsEnabled = true;
                        this.rnudQuantidade.IsEnabled = true;
                        this.rnudValorUnit.IsEnabled = true;
                        this.txtObservacoes.IsEnabled = true;
                        this.txtObservacoesItens.IsEnabled = true;

                        this.cbGola.Text = string.Empty;
                        this.cbMalha.Text = string.Empty;
                        this.cbProduto.Text = string.Empty;
                        this.cbTamanho.Text = string.Empty;
                        this.rnudQuantidade.Value = null;
                        this.rnudValorUnit.Value = null;
                        this.txtObservacoes.Text = string.Empty;

                        this.btnNovo.IsEnabled = false;
                        this.btnEditar.IsEnabled = false;
                        this.btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;

                        this.btnCancelarVenda.IsEnabled = true;
                        this.btnPesquisarCliente.IsEnabled = false;
                        this.btnProcurarImagem.IsEnabled = true;
                        this.btnAddItem.IsEnabled = true;
                        this.btnSalvarVenda.IsEnabled = true;

                        //Pesquisar venda para edição
                        PesquisaVendaUI pesq = new PesquisaVendaUI();
                        pesq.ShowDialog();

                        if (pesq.Venda.cd_Venda != 0) //Se foi selecionado alguma venda
                        {
                            Pessoa = PessoaDAL.RecuperarPessoa(pesq.Venda.cd_Pessoa);

                            ListaItensVenda.Clear();
                            ListaItensVendaAux.Clear();
                            ListaItensVendaAux = VendaDAL.RecuperarListaItensVenda(pesq.Venda.cd_Venda).OrderBy(a => a.PRODUTO.ds_OrdemExibicao).ThenBy(a => a.TAMANHO.ds_OrdemExibicao).ToList();

                            foreach (var item in ListaItensVendaAux)
                                ListaItensVenda.Add(item);

                            dtItensVenda.Rebind();

                            Venda = pesq.Venda;
                            ImagemVenda = VendaDAL.RecuperarImagemVenda(Venda.cd_Venda);

                            if (ImagemVenda.ds_Imagem.Length > 100)
                            {
                                BitmapImage btm;
                                using (MemoryStream ms = new MemoryStream(ImagemVenda.ds_Imagem.ToArray()))
                                {
                                    DadosImagem = ImagemVenda.ds_Imagem.ToArray();
                                    btm = new BitmapImage();
                                    btm.BeginInit();
                                    btm.StreamSource = ms;
                                    btm.CacheOption = BitmapCacheOption.OnLoad;
                                    btm.EndInit();

                                    ImagemCamiseta.Source = btm;
                                }
                            }

                            if (VendaDAL.VerificarEdicaoVenda(Venda.cd_Venda) != true)
                            {
                                if (MessageBox.Show(Mensagens.edicaoVendaRelacionada, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                                    VendaDAL.ExcluirCascataVenda(Venda.cd_Venda);
                                else
                                    this.btnSalvarVenda.IsEnabled = false;
                            }

                            this.LimparCamposItensVenda();

                            this.dpDataPrevista.IsEnabled = true;
                        }

                        else
                            VerificarOperacao(EnumTipoOperacao.Aguardar);

                        this.tipoOperacao = EnumTipoOperacao.Editar;
                        break;

                    case EnumTipoOperacao.Excluir:
                        if (MessageBox.Show(Mensagens.desejaRealmenteExcluirVenda, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            VendaDAL.ExcluirVenda(this.Venda);

                        new MensagemUI(Mensagens.registroExcluidoComSucesso).Show();
                        VerificarOperacao(EnumTipoOperacao.Aguardar);
                        break;

                    case EnumTipoOperacao.Cancelar:
                        this.cbGola.IsEnabled = false;
                        this.cbMalha.IsEnabled = false;
                        this.cbProduto.IsEnabled = false;
                        this.cbTamanho.IsEnabled = false;
                        this.rnudQuantidade.IsEnabled = false;
                        this.rnudValorUnit.IsEnabled = false;
                        this.txtObservacoes.IsEnabled = false;
                        this.txtObservacoesItens.IsEnabled = false;

                        this.cbGola.Text = string.Empty;
                        this.cbMalha.Text = string.Empty;
                        this.cbProduto.Text = string.Empty;
                        this.cbTamanho.Text = string.Empty;
                        this.rnudQuantidade.Value = null;
                        this.rnudValorUnit.Value = null;
                        this.txtObservacoes.Text = string.Empty;

                        this.txtNome.Text = string.Empty;

                        this.btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                        this.btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                        this.btnExcluir.IsEnabled = false;

                        this.btnCancelarVenda.IsEnabled = false;
                        this.btnPesquisarCliente.IsEnabled = false;
                        this.btnProcurarImagem.IsEnabled = false;
                        this.btnAddItem.IsEnabled = false;
                        this.btnSalvarVenda.IsEnabled = false;

                        this.DadosImagem = null;

                        this.ImagemCamiseta.Source = new BitmapImage(new Uri(@"/Imagens/ImagemCamisa.jpg", UriKind.Relative));

                        this.ListaItensVenda.Clear();
                        this.dtItensVenda.Rebind();

                        this.LimparValidacaoCampos();

                        this.LimparCamposItensVenda();

                        this.dpDataPrevista.Text = string.Empty;
                        this.dpDataPrevista.IsEnabled = true;

                        this.Venda = new VENDA();

                        this.tipoOperacao = EnumTipoOperacao.Cancelar;
                        break;

                    case EnumTipoOperacao.Aguardar:
                        this.cbGola.IsEnabled = false;
                        this.cbMalha.IsEnabled = false;
                        this.cbProduto.IsEnabled = false;
                        this.cbTamanho.IsEnabled = false;
                        this.rnudQuantidade.IsEnabled = false;
                        this.rnudValorUnit.IsEnabled = false;
                        this.txtObservacoes.IsEnabled = false;
                        this.txtObservacoesItens.IsEnabled = false;

                        this.cbGola.Text = string.Empty;
                        this.cbMalha.Text = string.Empty;
                        this.cbProduto.Text = string.Empty;
                        this.cbTamanho.Text = string.Empty;
                        this.rnudQuantidade.Value = null;
                        this.rnudValorUnit.Value = null;
                        this.txtObservacoes.Text = string.Empty;
                        this.txtObservacoesItens.Text = string.Empty;

                        this.txtNome.Text = string.Empty;

                        this.btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                        this.btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                        this.btnExcluir.IsEnabled = false;

                        this.btnCancelarVenda.IsEnabled = false;
                        this.btnPesquisarCliente.IsEnabled = false;
                        this.btnProcurarImagem.IsEnabled = false;
                        this.btnAddItem.IsEnabled = false;
                        this.btnSalvarVenda.IsEnabled = false;

                        this.DadosImagem = null;

                        this.ImagemCamiseta.Source = new BitmapImage(new Uri(@"/Imagens/ImagemCamisa.jpg", UriKind.Relative));

                        this.ListaItensVenda.Clear();
                        this.dtItensVenda.Rebind();

                        this.LimparValidacaoCampos();

                        this.LimparCamposItensVenda();

                        this.dpDataPrevista.Text = string.Empty;
                        this.dpDataPrevista.IsEnabled = true;

                        this.Venda = new VENDA();

                        this.tipoOperacao = EnumTipoOperacao.Aguardar;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                btnSalvarVenda.IsEnabled = false;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public void LimparCamposItensVenda()
        {
            this.rnudQuantidade.Value = null;
            this.cbProduto.Text = string.Empty;
            this.cbTamanho.Text = string.Empty;
            this.cbMalha.Text = string.Empty;
            this.cbGola.Text = string.Empty;
            this.rnudValorUnit.Value = null;
            this.txtObservacoesItens.Text = string.Empty;
            this.ItensVendaRecuperado = null;
        }

        #endregion
    }
}