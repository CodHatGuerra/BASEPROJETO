using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace NaMidia.UI
{
    public partial class UsuarioUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

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

        private List<LOGIN> listaLogin;
        public List<LOGIN> ListaLogin
        {
            get { return listaLogin; }
            set
            {
                listaLogin = value;
                OnPropertyChanged("listaLogin");
            }
        }

        private List<FUNCIONARIO> listaFuncionario;
        public List<FUNCIONARIO> ListaFuncionario
        {
            get { return listaFuncionario; }
            set
            {
                listaFuncionario = value;
                OnPropertyChanged("listaFuncionario");
            }
        }

        private List<PerfilUsuarioCombo> listaPerfil;
        public List<PerfilUsuarioCombo> ListaPerfil
        {
            get { return listaPerfil; }
            set
            {
                listaPerfil = value;
                OnPropertyChanged("ListaPerfil");
            }
        }

        bool perfilAdministrador = false;

        #endregion

        #region [ Construtor ]

        public UsuarioUI()
        {
            var lista = LoginDAL.RecuperarListaPerfilUsuario(Gerenciador.UsuarioAtivo.cd_Login);
            if (lista.Where(a => a.cd_Perfil == (int)EnumPerfil.Administrador && a.isChecked).Count() > 0)
                perfilAdministrador = true;

            ListaLogin = perfilAdministrador ?
                         LoginDAL.RecuperarListaLogin() :
                         LoginDAL.RecuperarListaLogin().Where(a => a.cd_Login == Gerenciador.UsuarioAtivo.cd_Login).ToList();

            ListaFuncionario = LoginDAL.RecuperarListaFuncionarios();
            ListaPerfil = new List<PerfilUsuarioCombo>();

            InitializeComponent();
            Loaded += frmCadastroUsuario_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void frmCadastroUsuario_Loaded(object sender, RoutedEventArgs e)
        {
            LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtUsuario.RowHeight);
            VerificarOperacao(EnumTipoOperacao.Aguardar);
        }

        private void dtUsuario_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                DataContext = LoginDAL.RecuperarLogin((dtUsuario.SelectedItem as LOGIN).cd_Login);
                txtSenha.Password = (DataContext as LOGIN).ds_Senha;
                ListaPerfil = LoginDAL.RecuperarListaPerfilUsuario((DataContext as LOGIN).cd_Login);
                this.btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbFuncionario_Loaded(object sender, RoutedEventArgs e)
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

        private void cbPerfil_Loaded(object sender, RoutedEventArgs e)
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
            VerificarOperacao(EnumTipoOperacao.Editar);
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    if (txtSenha.Password != (DataContext as LOGIN).ds_Senha)
                        (DataContext as LOGIN).ds_Senha = txtSenha.Password;

                    else
                        (DataContext as LOGIN).ds_Senha = Criptografia.Descriptografar(txtSenha.Password, true);

                    LoginDAL.AlterarLogin((DataContext as LOGIN), ListaPerfil);

                    //Atualizar a lista do RadGridView
                    ListaLogin = perfilAdministrador ?
                                LoginDAL.RecuperarListaLogin().OrderBy(a => a.ds_Usuario).ToList() :
                                LoginDAL.RecuperarListaLogin().Where(a => a.cd_Login == Gerenciador.UsuarioAtivo.cd_Login).OrderBy(a => a.ds_Usuario).ToList();

                    VerificarOperacao(EnumTipoOperacao.Aguardar);
                    new MensagemUI(Mensagens.registroSalvoComSucesso).Show();
                }
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

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VerificarOperacao(EnumTipoOperacao.Excluir);
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

            if (Convert.ToInt32(cbFuncionario.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbFuncionario.SetaValidacao("CAMPO_OBRIGATORIO");
                cbFuncionario.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbFuncionario.Text))
            {
                cbFuncionario.SetaValidacao("CAMPO_OBRIGATORIO");
                cbFuncionario.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbFuncionario.LimpaErrosValidacao();

            if (ListaPerfil.Where(a => a.isChecked == true).Count() == 0)
            {
                if (Convert.ToInt32(cbPerfil.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
                {
                    cbPerfil.SetaValidacao("CAMPO_OBRIGATORIO");
                    cbPerfil.RaiseErroValidacao();
                    formularioValidado = false;
                }

                else if (string.IsNullOrEmpty(cbPerfil.Text))
                {
                    cbPerfil.SetaValidacao("CAMPO_OBRIGATORIO");
                    cbPerfil.RaiseErroValidacao();
                    formularioValidado = false;
                }

                else
                    cbPerfil.LimpaErrosValidacao();
            }

            if (string.IsNullOrEmpty(txtNome.Text))
            {
                txtNome.SetaValidacao("CAMPO_OBRIGATORIO");
                txtNome.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtNome.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtSenha.Password))
            {
                txtSenha.SetaValidacao("CAMPO_OBRIGATORIO");
                txtSenha.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtSenha.LimpaErrosValidacao();

            if (LoginDAL.VerificarUsuarioExiste(DataContext as LOGIN))
            {
                MessageBox.Show(Mensagens.usuarioExiste, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                formularioValidado = false;
            }

            if (LoginDAL.VerificarFuncionarioJaRelacionado(DataContext as LOGIN))
            {
                MessageBox.Show(Mensagens.funcionarioJaRelacionado, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
                formularioValidado = false;
            }

            return formularioValidado;
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                case EnumTipoOperacao.Novo:
                    txtNome.Clear();
                    txtNome.IsEnabled = true;

                    txtSenha.Clear();
                    txtSenha.IsEnabled = true;

                    cbFuncionario.Text = string.Empty;
                    cbFuncionario.IsEnabled = true;

                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtUsuario.IsEnabled = false;
                    cbPerfil.IsEnabled = true;

                    ListaPerfil = LoginDAL.RecuperarListaPerfilUsuario(0);

                    DataContext = new LOGIN();
                    break;

                case EnumTipoOperacao.Editar:
                    txtNome.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    txtSenha.IsEnabled = true;

                    cbFuncionario.IsEnabled = perfilAdministrador ? true : false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    cbPerfil.IsEnabled = perfilAdministrador ? true : false;
                    dtUsuario.IsEnabled = true;
                    break;


                case EnumTipoOperacao.Excluir:

                    if ((DataContext as LOGIN).cd_Login != 0)
                    {
                        if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            LoginDAL.ExcluirLogin((DataContext as LOGIN).cd_Login);

                            ListaLogin = perfilAdministrador ?
                               LoginDAL.RecuperarListaLogin().OrderBy(a => a.ds_Usuario).ToList() :
                               LoginDAL.RecuperarListaLogin().Where(a => a.cd_Login == Gerenciador.UsuarioAtivo.cd_Login).OrderBy(a => a.ds_Usuario).ToList();

                            VerificarOperacao(EnumTipoOperacao.Aguardar);
                            new MensagemUI(Mensagens.registroExcluidoComSucesso).Show();
                        }
                    }

                    else
                        MessageBox.Show(Mensagens.selecioneUmRegistro);
                    break;

                case EnumTipoOperacao.Cancelar:
                    txtNome.IsEnabled = false;
                    txtNome.Clear();
                    txtNome.LimpaErrosValidacao();

                    txtSenha.Clear();
                    txtSenha.LimpaErrosValidacao();
                    txtSenha.IsEnabled = false;

                    cbFuncionario.Text = string.Empty;
                    cbFuncionario.IsEnabled = false;
                    cbFuncionario.LimpaErrosValidacao();

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtUsuario.IsEnabled = true;
                    dtUsuario.SelectedItem = dtUsuario.Items[0];
                    dtUsuario_SelectionChanged(null, null);

                    cbPerfil.IsEnabled = false;
                    txtNome.LimpaErrosValidacao();
                    break;

                case EnumTipoOperacao.Aguardar:
                    txtNome.IsEnabled = false;
                    txtNome.Clear();
                    txtNome.LimpaErrosValidacao();

                    txtSenha.Clear();
                    txtSenha.LimpaErrosValidacao();
                    txtSenha.IsEnabled = false;

                    cbFuncionario.Text = string.Empty;
                    cbFuncionario.IsEnabled = false;
                    cbFuncionario.LimpaErrosValidacao();

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    cbPerfil.IsEnabled = false;

                    dtUsuario.IsEnabled = true;
                    dtUsuario.SelectedItem = dtUsuario.Items[0];
                    dtUsuario_SelectionChanged(null, null);

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
