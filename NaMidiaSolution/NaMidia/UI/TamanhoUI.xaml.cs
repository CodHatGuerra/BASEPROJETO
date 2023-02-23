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
    public partial class TamanhoUI : UserControl, INotifyPropertyChanged
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

        List<TAMANHO> listaTamanho;
        public List<TAMANHO> ListaTamanho
        {
            get { return listaTamanho; }
            set
            {
                listaTamanho = value;
                OnPropertyChanged("listaTamanho");
            }
        }

        TAMANHO tamanho;
        public TAMANHO Tamanho
        {
            get
            {
                return tamanho;
            }

            set
            {
                tamanho = value;
                this.OnPropertyChanged("Tamanho");
            }
        }

        #endregion

        #region [ Construtor ]

        public TamanhoUI()
        {
            this.ListaTamanho = new List<TAMANHO>();
            this.LinhasGridView = 20;

            InitializeComponent();
            Loaded += frmCadastroTamanho_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void frmCadastroTamanho_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaTamanho = TamanhoDAL.RecuperarListaTamanhos();
            }).ContinueWith(task =>
            {
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtTamanho.RowHeight);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void dtTamanho_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtTamanho.SelectedItem != null)
                {
                    Tamanho = TamanhoDAL.RecuperarTamanho((dtTamanho.SelectedItem as TAMANHO).cd_Tamanho);
                    btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    TamanhoDAL.AlterarTamanho(Tamanho);

                    ListaTamanho = TamanhoDAL.RecuperarListaTamanhos();

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
                if (Tamanho.cd_Tamanho != 0)
                {
                    if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        TamanhoDAL.ExcluirTamanho(Tamanho.cd_Tamanho);
                        ListaTamanho = TamanhoDAL.RecuperarListaTamanhos();
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

            if (string.IsNullOrEmpty(rnudOrdem.Value.ToString()))
            {
                txtNome.SetaValidacao("CAMPO_OBRIGATORIO");
                txtNome.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                rnudOrdem.LimpaErrosValidacao();

            return formularioValidado;
        }

        public void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                case EnumTipoOperacao.Novo:
                    txtNome.Clear();
                    txtNome.IsEnabled = true;

                    rnudOrdem.Value = 0;
                    rnudOrdem.IsEnabled = true;

                    btnEditar.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtTamanho.IsEnabled = false;

                    Tamanho = new TAMANHO();
                    break;

                case EnumTipoOperacao.Editar:
                    txtNome.IsEnabled = true;

                    rnudOrdem.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    dtTamanho.IsEnabled = true;
                    break;

                case EnumTipoOperacao.Cancelar:
                    txtNome.IsEnabled = false;
                    txtNome.Clear();
                    txtNome.LimpaErrosValidacao();

                    rnudOrdem.Value = 0;
                    rnudOrdem.IsEnabled = true;
                    rnudOrdem.LimpaErrosValidacao();

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtTamanho.IsEnabled = true;

                    txtNome.LimpaErrosValidacao();

                    break;

                case EnumTipoOperacao.Aguardar:
                    txtNome.Clear();
                    txtNome.IsEnabled = false;

                    rnudOrdem.Value = 0;
                    rnudOrdem.IsEnabled = false;

                    btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    dtTamanho.IsEnabled = true;

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
