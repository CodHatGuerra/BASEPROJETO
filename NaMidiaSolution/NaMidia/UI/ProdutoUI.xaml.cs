using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace NaMidia.UI
{
    public partial class ProdutoUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

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

        List<PRODUTO> listaProduto;
        public List<PRODUTO> ListaProduto
        {
            get { return listaProduto; }
            set
            {
                listaProduto = value;
                OnPropertyChanged("listaProduto");
            }
        }

        private List<CATEGORIA> listaCategoria;

        public List<CATEGORIA> ListaCategoria
        {
            get
            {
                return listaCategoria;
            }

            set
            {
                listaCategoria = value;
                OnPropertyChanged("ListaCategoria");
            }
        }

        private List<FORNECEDOR> listaFornecedor;
        public List<FORNECEDOR> ListaFornecedor
        {
            get
            {
                return listaFornecedor;
            }

            set
            {
                listaFornecedor = value;
                OnPropertyChanged("ListaFornecedor");
            }

        }

        PRODUTO produto;
        public PRODUTO Produto
        {
            get { return produto; }
            set
            {
                produto = value;
                this.OnPropertyChanged("Produto");
            }
        }

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

        #endregion

        #region [ Construtor ]

        public ProdutoUI()
        {
            this.ListaProduto = new List<PRODUTO>();
            this.LinhasGridView = 20;

            InitializeComponent();

            Loaded += frmCadastroProduto_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void frmCadastroProduto_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaProduto = ProdutoDAL.RecuperarListaProdutos();
                this.ListaCategoria = CategoriaDAL.RecuperarListaCategoria();
                this.ListaFornecedor = FornecedorDAL.RecuperarListaFornecedor();


            }).ContinueWith(task =>
            {
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtProduto.RowHeight);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void dtProduto_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtProduto.SelectedItem != null)
                {
                    this.Produto = ProdutoDAL.RecuperarProduto((dtProduto.SelectedItem as PRODUTO).cd_Produto);
                    this.cbCategoria.SelectedValue = this.Produto.cd_Categoria == null ? CODIGO_COMBO_OPCAO_PADRAO : this.Produto.cd_Categoria;
                    this.cbFornecedor.SelectedValue = this.Produto.cd_Fornecedor == null ? CODIGO_COMBO_OPCAO_PADRAO : this.Produto.cd_Fornecedor;
                    this.btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void rnudPrecoCusto_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (rnudPrecoCusto.Value != 0)
            {
                rnudPorcentagem.ValueChanged -= rnudPorcentagem_ValueChanged;
                rnudPorcentagem.Value = ((rnudPrecoVenda.Value - rnudPrecoCusto.Value) / rnudPrecoCusto.Value) * 100;
                rnudPorcentagem.ValueChanged += rnudPorcentagem_ValueChanged;
            }
        }

        private void rnudPrecoVenda_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (rnudPrecoCusto.Value != 0)
            {
                rnudPorcentagem.ValueChanged -= rnudPorcentagem_ValueChanged;
                rnudPorcentagem.Value = ((rnudPrecoVenda.Value - rnudPrecoCusto.Value) / rnudPrecoCusto.Value) * 100;
                rnudPorcentagem.ValueChanged += rnudPorcentagem_ValueChanged;
            }
        }

        private void rnudPorcentagem_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            rnudPrecoVenda.Value = (rnudPrecoCusto.Value * (rnudPorcentagem.Value / 100)) + rnudPrecoCusto.Value;
        }

        #region [ Botões ]

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Novo);
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Editar);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Cancelar);
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarCampos())
                {
                    ProdutoDAL.AlterarProduto(this.Produto);

                    ListaProduto = ProdutoDAL.RecuperarListaProdutos();

                    VerificarOperacao(EnumTipoOperacao.Aguardar);
                    new MensagemUI(Mensagens.registroSalvoComSucesso).Show();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((this.Produto).cd_Produto != 0)
                {
                    if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ProdutoDAL.ExcluirProduto(this.Produto.cd_Produto);
                        ListaProduto = ProdutoDAL.RecuperarListaProdutos();
                        VerificarOperacao(EnumTipoOperacao.Aguardar);
                        new MensagemUI(Mensagens.registroExcluidoComSucesso).Show();
                    }
                }

                else
                    MessageBox.Show(Mensagens.selecioneUmRegistro);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [ Combobox ]

        private void cbCategoria_Loaded(object sender, RoutedEventArgs e)
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

        private void cbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbCategoria.SelectedValue != null)
                {
                    this.Produto.cd_Categoria = Convert.ToInt32(cbCategoria.SelectedValue);
                }
            }
            catch { }
        }

        private void cbFornecedor_Loaded(object sender, RoutedEventArgs e)
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

        private void cbFornecedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbFornecedor.SelectedValue != null)
                {
                    this.Produto.cd_Fornecedor = Convert.ToInt32(cbFornecedor.SelectedValue);
                }
            }
            catch { }
        }

        #endregion

        #endregion

        #region [ Metodos ]

        public bool validarCampos()
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

            if (string.IsNullOrEmpty(rnudOrdem.Value.ToString()))
            {
                rnudOrdem.SetaValidacao("CAMPO_OBRIGATORIO");
                rnudOrdem.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                rnudOrdem.LimpaErrosValidacao();

            //if (string.IsNullOrEmpty(rnudEstoqueAtual.Value.ToString()))
            //{
            //    rnudEstoqueAtual.SetaValidacao("CAMPO_OBRIGATORIO");
            //    rnudEstoqueAtual.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else
            //    rnudEstoqueAtual.LimpaErrosValidacao();

            //if (string.IsNullOrEmpty(rnudEstoqueMinimo.Value.ToString()))
            //{
            //    rnudEstoqueMinimo.SetaValidacao("CAMPO_OBRIGATORIO");
            //    rnudEstoqueMinimo.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else
            //    rnudEstoqueMinimo.LimpaErrosValidacao();

            //if (string.IsNullOrEmpty(rnudPrecoCusto.Value.ToString()))
            //{
            //    rnudPrecoCusto.SetaValidacao("CAMPO_OBRIGATORIO");
            //    rnudPrecoCusto.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else
            //    rnudPrecoCusto.LimpaErrosValidacao();

            //if (string.IsNullOrEmpty(rnudPrecoVenda.Value.ToString()))
            //{
            //    rnudPrecoVenda.SetaValidacao("CAMPO_OBRIGATORIO");
            //    rnudPrecoVenda.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else
            //    rnudPrecoVenda.LimpaErrosValidacao();

            //if (Convert.ToInt32(cbCategoria.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            //{
            //    cbCategoria.SetaValidacao("CAMPO_OBRIGATORIO");
            //    cbCategoria.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else if (string.IsNullOrEmpty(cbCategoria.Text))
            //{
            //    cbCategoria.SetaValidacao("CAMPO_OBRIGATORIO");
            //    cbCategoria.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else
            //    cbCategoria.LimpaErrosValidacao();

            //if (Convert.ToInt32(cbFornecedor.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            //{
            //    cbFornecedor.SetaValidacao("CAMPO_OBRIGATORIO");
            //    cbFornecedor.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else if (string.IsNullOrEmpty(cbFornecedor.Text))
            //{
            //    cbFornecedor.SetaValidacao("CAMPO_OBRIGATORIO");
            //    cbFornecedor.RaiseErroValidacao();
            //    formularioValidado = false;
            //}

            //else
            //    cbFornecedor.LimpaErrosValidacao();

            return formularioValidado;
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                case EnumTipoOperacao.Novo:
                    txtNome.Clear();
                    txtNome.IsEnabled = true;

                    rnudEstoqueAtual.Value = 0;
                    rnudEstoqueMinimo.Value = 0;
                    rnudPrecoCusto.Value = 0;
                    rnudPrecoVenda.Value = 0;
                    rnudOrdem.Value = 0;

                    rnudEstoqueAtual.IsEnabled = true;
                    rnudEstoqueMinimo.IsEnabled = true;
                    rnudPrecoCusto.IsEnabled = true;
                    rnudPrecoVenda.IsEnabled = true;
                    rnudPorcentagem.IsEnabled = true;
                    rnudOrdem.IsEnabled = true;

                    cbCategoria.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
                    cbFornecedor.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;

                    cbCategoria.Text = string.Empty;
                    cbFornecedor.Text = string.Empty;

                    cbCategoria.IsEnabled = true;
                    cbFornecedor.IsEnabled = true;

                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtProduto.IsEnabled = false;

                    this.Produto = new PRODUTO();
                    break;

                case EnumTipoOperacao.Editar:
                    txtNome.IsEnabled = true;

                    rnudEstoqueAtual.IsEnabled = true;
                    rnudEstoqueMinimo.IsEnabled = true;
                    rnudPrecoCusto.IsEnabled = true;
                    rnudPrecoVenda.IsEnabled = true;
                    rnudPorcentagem.IsEnabled = true;
                    rnudOrdem.IsEnabled = true;

                    cbCategoria.IsEnabled = true;
                    cbFornecedor.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtProduto.IsEnabled = true;
                    break;

                case EnumTipoOperacao.Cancelar:
                    txtNome.IsEnabled = false;
                    txtNome.Clear();
                    txtNome.LimpaErrosValidacao();

                    rnudEstoqueAtual.IsEnabled = false;
                    rnudEstoqueMinimo.IsEnabled = false;
                    rnudPrecoCusto.IsEnabled = false;
                    rnudPrecoVenda.IsEnabled = false;
                    rnudPorcentagem.IsEnabled = false;
                    rnudOrdem.IsEnabled = false;

                    rnudEstoqueAtual.Value = 0;
                    rnudEstoqueMinimo.Value = 0;
                    rnudPrecoCusto.Value = 0;
                    rnudPrecoVenda.Value = 0;
                    rnudPorcentagem.Value = 0;
                    rnudOrdem.Value = 0;

                    cbCategoria.IsEnabled = false;
                    cbCategoria.Text = string.Empty;
                    cbFornecedor.IsEnabled = false;
                    cbFornecedor.Text = string.Empty;

                    rnudEstoqueAtual.LimpaErrosValidacao();

                    rnudEstoqueAtual.LimpaErrosValidacao();
                    rnudEstoqueMinimo.LimpaErrosValidacao();
                    rnudPrecoCusto.LimpaErrosValidacao();
                    rnudPrecoVenda.LimpaErrosValidacao();
                    cbCategoria.LimpaErrosValidacao();
                    cbFornecedor.LimpaErrosValidacao();

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtProduto.IsEnabled = true;

                    txtNome.LimpaErrosValidacao();
                    this.Produto = new PRODUTO();

                    break;

                case EnumTipoOperacao.Aguardar:
                    txtNome.Clear();
                    txtNome.IsEnabled = false;

                    rnudEstoqueAtual.IsEnabled = false;
                    rnudEstoqueMinimo.IsEnabled = false;
                    rnudPrecoCusto.IsEnabled = false;
                    rnudPrecoVenda.IsEnabled = false;
                    rnudPorcentagem.IsEnabled = false;
                    rnudOrdem.IsEnabled = false;

                    rnudEstoqueAtual.Value = 0;
                    rnudEstoqueMinimo.Value = 0;
                    rnudPrecoCusto.Value = 0;
                    rnudPrecoVenda.Value = 0;
                    rnudPorcentagem.Value = 0;
                    rnudOrdem.Value = 0;

                    cbCategoria.IsEnabled = false;
                    cbCategoria.Text = string.Empty;
                    cbFornecedor.IsEnabled = false;
                    cbFornecedor.Text = string.Empty;

                    rnudEstoqueAtual.LimpaErrosValidacao();
                    rnudEstoqueMinimo.LimpaErrosValidacao();
                    rnudPrecoCusto.LimpaErrosValidacao();
                    rnudPrecoVenda.LimpaErrosValidacao();
                    rnudOrdem.LimpaErrosValidacao();

                    cbCategoria.LimpaErrosValidacao();
                    cbFornecedor.LimpaErrosValidacao();

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtProduto.IsEnabled = true;

                    txtNome.LimpaErrosValidacao();

                    this.Produto = new PRODUTO();
                    break;
            }
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
