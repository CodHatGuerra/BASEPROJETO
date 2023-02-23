using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace NaMidia
{
    public partial class LoginUI : RadWindow
    {
        #region [ Propriedades ]

        bool teclaEnter = true;

        #endregion

        #region [ Construtor ]

        public LoginUI()
        {
            InitializeComponent();
            Loaded += LoginUI_Loaded;
            txtUsuario.Focus();
            KeyUp += LoginUI_KeyUp;
            txtUsuario.GotKeyboardFocus += txtUsuario_GotKeyboardFocus;
            txtSenha.GotKeyboardFocus += txtSenha_GotKeyboardFocus;
        }

        private void LoginUI_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }
        }

        #endregion

        #region [ Eventos ]

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarCampos())
                {
                    Gerenciador.UsuarioAtivo = LoginDAL.VerificarLogin(txtUsuario.Text, txtSenha.Password);

                    if (Gerenciador.UsuarioAtivo == null)
                    {
                        MessageBox.Show(Mensagens.usuarioOuSenhaInvalidos, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        this.teclaEnter = false;
                    }

                    else
                        Processo();
                }
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoginUI_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.teclaEnter)
            {
                this.btnEntrar.Focus();
                this.btnEntrar_Click(sender, e);
            }
        }

        private void txtSenha_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.teclaEnter = true;
        }

        private void txtUsuario_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.teclaEnter = true;
        }

        #endregion

        #region [ Metodos ]

        private bool validarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                txtUsuario.SetaValidacao("CAMPO_OBRIGATORIO");
                txtUsuario.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtUsuario.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtSenha.Password))
            {
                txtSenha.SetaValidacao("CAMPO_OBRIGATORIO");
                txtSenha.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtSenha.LimpaErrosValidacao();

            return formularioValidado;
        }

        private void Processo()
        {
            btnEntrar.IsEnabled = false;
            Gerenciador.DicModuloAcao = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();

            MainWindow mainWindow = new MainWindow(0);
            mainWindow.Show();

            this.Close();
        }

        #endregion
    }
}
