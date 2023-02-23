using NaMidia.Classes;
using NaMidia.UI;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace NaMidia
{
    public partial class FuncionarioUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

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

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

        List<FUNCIONARIO> listaFuncionario;
        public List<FUNCIONARIO> ListaFuncionario
        {
            get { return listaFuncionario; }
            set
            {
                listaFuncionario = value;
                OnPropertyChanged("listaFuncionario");
            }
        }

        List<CIDADE> listaCidade;
        public List<CIDADE> ListaCidade
        {
            get { return listaCidade; }
            set
            {
                listaCidade = value;
                OnPropertyChanged("listaCidade");
            }
        }

        List<CARGO> listaCargo;
        public List<CARGO> ListaCargo
        {
            get { return listaCargo; }
            set
            {
                listaCargo = value;
                OnPropertyChanged("listaCargo");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        FUNCIONARIO funcionario;
        public FUNCIONARIO Funcionario
        {
            get { return funcionario; }
            set
            {
                funcionario = value;
                this.OnPropertyChanged("funcionario");
            }
        }

        #endregion

        #region [ Construtor ]

        public FuncionarioUI()
        {
            this.ListaCidade = new List<CIDADE>();
            this.ListaFuncionario = new List<FUNCIONARIO>();
            this.ListaCargo = new List<CARGO>();
            this.LinhasGridView = 20;

            InitializeComponent();
            Loaded += frmFuncionario_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void frmFuncionario_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaCidade = CidadeDAL.RecuperarListaCidade();
                this.ListaFuncionario = FuncionarioDAL.RecuperarListaFuncionario();
                this.ListaCargo = FuncionarioDAL.RecuperarListaCargo();
            }).ContinueWith(task =>
            {
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtFuncionario.RowHeight);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());


        }

        private void dtFuncionario_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtFuncionario.SelectedItem != null)
                    Funcionario = FuncionarioDAL.RecuperarFuncionario((dtFuncionario.SelectedItem as FUNCIONARIO).cd_Funcionario);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void sldStatus_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sldStatus.Value == 1)
                sldStatus.Background = new SolidColorBrush(Colors.Green);

            else if (sldStatus.Value == 0)
                sldStatus.Background = new SolidColorBrush(Colors.Red);
        }

        private void cbCidade_Loaded(object sender, RoutedEventArgs e)
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

        private void cbCargo_Loaded(object sender, RoutedEventArgs e)
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
                if (validarCampos())
                {
                    FuncionarioDAL.AlterarFuncionario(Funcionario);

                    ListaFuncionario = FuncionarioDAL.RecuperarListaFuncionario();
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

            if (string.IsNullOrEmpty(txtEndereco.Text))
            {
                txtEndereco.SetaValidacao("CAMPO_OBRIGATORIO");
                txtEndereco.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtEndereco.LimpaErrosValidacao();

            if (Convert.ToInt32(cbCidade.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbCidade.SetaValidacao("CAMPO_OBRIGATORIO");
                cbCidade.RaiseErroValidacao();
                formularioValidado = false;
            }

            else if (string.IsNullOrEmpty(cbCidade.Text))
            {
                cbCidade.SetaValidacao("CAMPO_OBRIGATORIO");
                cbCidade.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbCidade.LimpaErrosValidacao();


            if (Convert.ToInt32(cbCargo.SelectedValue) == CODIGO_COMBO_OPCAO_PADRAO)
            {
                cbCargo.SetaValidacao("CAMPO_OBRIGATORIO");
                cbCargo.RaiseErroValidacao();
                formularioValidado = false;
            }


            else if (string.IsNullOrEmpty(cbCargo.Text))
            {
                cbCargo.SetaValidacao("CAMPO_OBRIGATORIO");
                cbCargo.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbCargo.LimpaErrosValidacao();

            return formularioValidado;
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                case EnumTipoOperacao.Novo:
                    txtBairro.Clear();
                    txtCPF.Clear();
                    txtEmail.Clear();
                    txtEndereco.Clear();
                    txtNome.Clear();
                    txtNumero.Clear();
                    txtRG.Clear();
                    txtSalario.Value = null;
                    txtTelefone.Clear();
                    cbCargo.Text = string.Empty;
                    cbCidade.Text = string.Empty;
                    dtEntrada.Text = string.Empty;
                    dtSaida.Text = string.Empty;

                    txtBairro.IsEnabled = true;
                    txtCPF.IsEnabled = true;
                    txtEmail.IsEnabled = true;
                    txtEndereco.IsEnabled = true;
                    txtNome.IsEnabled = true;
                    txtNumero.IsEnabled = true;
                    txtRG.IsEnabled = true;
                    txtSalario.IsEnabled = true;
                    txtTelefone.IsEnabled = true;
                    cbCargo.IsEnabled = true;
                    cbCidade.IsEnabled = true;
                    dtEntrada.IsEnabled = true;
                    dtSaida.IsEnabled = true;

                    btnEditar.IsEnabled = false;
                    btnNovo.IsEnabled = true;
                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtFuncionario.IsEnabled = false;

                    Funcionario = new FUNCIONARIO();
                    Funcionario.statusFuncionario = true;

                    break;

                case EnumTipoOperacao.Editar:
                    txtBairro.IsEnabled = true;
                    txtCPF.IsEnabled = true;
                    txtEmail.IsEnabled = true;
                    txtEndereco.IsEnabled = true;
                    txtNome.IsEnabled = true;
                    txtNumero.IsEnabled = true;
                    txtRG.IsEnabled = true;
                    txtSalario.IsEnabled = true;
                    txtTelefone.IsEnabled = true;
                    cbCargo.IsEnabled = true;
                    cbCidade.IsEnabled = true;
                    dtEntrada.IsEnabled = true;
                    dtSaida.IsEnabled = true;

                    btnEditar.IsEnabled = true;
                    btnNovo.IsEnabled = false;
                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtFuncionario.IsEnabled = true;

                    break;

                case EnumTipoOperacao.Cancelar:
                    txtBairro.Clear();
                    txtCPF.Clear();
                    txtEmail.Clear();
                    txtEndereco.Clear();
                    txtNome.Clear();
                    txtNumero.Clear();
                    txtRG.Clear();
                    txtSalario.Value = null;
                    txtTelefone.Clear();
                    cbCargo.Text = string.Empty;
                    cbCidade.Text = string.Empty;
                    dtEntrada.Text = string.Empty;
                    dtSaida.Text = string.Empty;

                    txtBairro.IsEnabled = false;
                    txtCPF.IsEnabled = false;
                    txtEmail.IsEnabled = false;
                    txtEndereco.IsEnabled = false;
                    txtNome.IsEnabled = false;
                    txtNumero.IsEnabled = false;
                    txtRG.IsEnabled = false;
                    txtSalario.IsEnabled = false;
                    txtTelefone.IsEnabled = false;
                    cbCargo.IsEnabled = false;
                    cbCidade.IsEnabled = false;
                    dtEntrada.IsEnabled = false;
                    dtSaida.IsEnabled = false;

                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtFuncionario.IsEnabled = true;

                    break;

                case EnumTipoOperacao.Aguardar:
                    txtBairro.Clear();
                    txtCPF.Clear();
                    txtEmail.Clear();
                    txtEndereco.Clear();
                    txtNome.Clear();
                    txtNumero.Clear();
                    txtRG.Clear();
                    txtSalario.Value = null;
                    txtTelefone.Clear();
                    cbCargo.Text = string.Empty;
                    cbCidade.Text = string.Empty;
                    dtEntrada.Text = string.Empty;
                    dtSaida.Text = string.Empty;

                    txtBairro.IsEnabled = false;
                    txtCPF.IsEnabled = false;
                    txtEmail.IsEnabled = false;
                    txtEndereco.IsEnabled = false;
                    txtNome.IsEnabled = false;
                    txtNumero.IsEnabled = false;
                    txtRG.IsEnabled = false;
                    txtSalario.IsEnabled = false;
                    txtTelefone.IsEnabled = false;
                    cbCargo.IsEnabled = false;
                    cbCidade.IsEnabled = false;
                    dtEntrada.IsEnabled = false;
                    dtSaida.IsEnabled = false;

                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtFuncionario.IsEnabled = true;

                    break;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            try
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(name));
            }
            catch (Exception) { }
        }

        #endregion
    }
}
