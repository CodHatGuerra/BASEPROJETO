using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NaMidia.UI
{
    public partial class MalhaUI : UserControl, INotifyPropertyChanged
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

        List<MALHA> listaMalha;
        public List<MALHA> ListaMalha
        {
            get { return listaMalha; }
            set
            {
                listaMalha = value;
                OnPropertyChanged("listaMalha");
            }
        }

        MALHA malha;
        public MALHA Malha
        {
            get
            {
                return malha;
            }

            set
            {
                malha = value;
                this.OnPropertyChanged("Malha");
            }
        }

        #endregion

        #region [ Construtor ]

        public MalhaUI()
        {
            this.ListaMalha = new List<MALHA>();
            this.LinhasGridView = 20;

            InitializeComponent();
            Loaded += frmCadastroMalha_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void frmCadastroMalha_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaMalha = MalhaDAL.RecuperarListaMalhas();
            }).ContinueWith(task =>
            {
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtMalha.RowHeight);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void dtMalha_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtMalha.SelectedItem != null)
                {
                    Malha = MalhaDAL.RecuperarMalha((dtMalha.SelectedItem as MALHA).cd_Malha);
                    btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region [ Botoes ]

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
                    MalhaDAL.AlterarMalha(Malha);
                    ListaMalha = MalhaDAL.RecuperarListaMalhas();

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
                if (Malha.cd_Malha != 0)
                {
                    if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        MalhaDAL.ExcluirMalha(Malha.cd_Malha);
                        ListaMalha = MalhaDAL.RecuperarListaMalhas();
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

            return formularioValidado;
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                case EnumTipoOperacao.Novo:
                    txtNome.Clear();
                    txtNome.IsEnabled = true;

                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtMalha.IsEnabled = false;
                    Malha = new MALHA();

                    break;

                case EnumTipoOperacao.Editar:
                    txtNome.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtMalha.IsEnabled = true;
                    break;

                case EnumTipoOperacao.Cancelar:
                    txtNome.IsEnabled = false;
                    txtNome.Clear();
                    txtNome.LimpaErrosValidacao();

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtMalha.IsEnabled = true;

                    txtNome.LimpaErrosValidacao();

                    break;

                case EnumTipoOperacao.Aguardar:
                    txtNome.Clear();
                    txtNome.IsEnabled = false;

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtMalha.IsEnabled = true;

                    txtNome.LimpaErrosValidacao();

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
