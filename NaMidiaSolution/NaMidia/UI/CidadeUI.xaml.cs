using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace NaMidia.UI
{
    public partial class CidadeUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

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

        public event PropertyChangedEventHandler PropertyChanged;

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

        private List<UF> listaUf;
        public List<UF> ListaUf
        {
            get { return this.listaUf; }
            set
            {
                this.listaUf = value;
            }
        }

        private List<CIDADE> listaCidade;
        public List<CIDADE> ListaCidade
        {
            get { return this.listaCidade; }
            set
            {
                this.listaCidade = value;
                OnPropertyChanged("listaCidade");
            }
        }

        CIDADE cidade;
        public CIDADE Cidade
        {
            get { return cidade; }
            set
            {
                cidade = value;
                OnPropertyChanged("Cidade");
            }
        }

        #endregion

        #region [ Construtor ]

        public CidadeUI()
        {
            this.ListaUf = CidadeDAL.RecuperarListaUF();
            this.ListaCidade = CidadeDAL.RecuperarListaCidade();
            this.LinhasGridView = 20;

            InitializeComponent();
            Loaded += frmCadastroCidade_Loaded;

            VerificarOperacao(EnumTipoOperacao.Aguardar);
        }

        #endregion

        #region [ Eventos UI ]

        private void frmCadastroCidade_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((this.rGridView.ActualHeight - 175) / this.dtCidade.RowHeight);
        }

        private void dtCidade_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (this.dtCidade.SelectedItem != null)
                {
                    this.Cidade = CidadeDAL.RecuperarCidade((dtCidade.SelectedItem as CIDADE).cd_Cidade);
                    this.btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbUf_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RadComboBox combo = sender as RadComboBox;
                combo.SelectedValue = CODIGO_COMBO_OPCAO_PADRAO;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        #region [ Botões ]

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Novo);
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (this.Cidade != null && this.Cidade.cd_Cidade != 0)
                VerificarOperacao(EnumTipoOperacao.Editar);
            else
                MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Information);
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    CidadeDAL.AlterarCidade(this.Cidade);

                    //Atualizando a Lista
                    ListaCidade = CidadeDAL.RecuperarListaCidade();

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
                if (this.Cidade.cd_Cidade != 0)
                {
                    if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        CidadeDAL.ExcluirCidade(this.Cidade.cd_Cidade);

                        //Atualizando a Lista
                        ListaCidade = CidadeDAL.RecuperarListaCidade();
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VerificarOperacao(EnumTipoOperacao.Cancelar);
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

            if (Convert.ToInt32(cbUf.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbUf.SetaValidacao("CAMPO_OBRIGATORIO");
                cbUf.RaiseErroValidacao();
                formularioValidado = false;
            }


            else if (string.IsNullOrEmpty(cbUf.Text))
            {
                cbUf.SetaValidacao("CAMPO_OBRIGATORIO");
                cbUf.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbUf.LimpaErrosValidacao();

            return formularioValidado;
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                case EnumTipoOperacao.Novo:
                    txtNome.Clear();
                    txtNome.IsEnabled = true;

                    cbUf.SelectedValue = null;
                    cbUf.IsEnabled = true;

                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;
                    dtCidade.IsEnabled = false;

                    this.Cidade = new CIDADE();

                    break;

                case EnumTipoOperacao.Editar:
                    txtNome.IsEnabled = true;

                    cbUf.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtCidade.IsEnabled = true;
                    break;

                case EnumTipoOperacao.Cancelar:
                    txtNome.Clear();
                    txtNome.IsEnabled = false;

                    cbUf.SelectedValue = null;
                    cbUf.IsEnabled = false;

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;
                    dtCidade.IsEnabled = true;

                    //Limpar Erros de validação
                    txtNome.LimpaErrosValidacao();
                    cbUf.LimpaErrosValidacao();

                    break;

                case EnumTipoOperacao.Aguardar:
                    txtNome.Clear();
                    txtNome.IsEnabled = false;

                    cbUf.SelectedValue = null;
                    cbUf.IsEnabled = false;

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;
                    dtCidade.IsEnabled = true;

                    //Limpar Erros de validação
                    txtNome.LimpaErrosValidacao();
                    cbUf.LimpaErrosValidacao();
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

