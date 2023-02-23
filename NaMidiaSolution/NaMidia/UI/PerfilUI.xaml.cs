using NaMidia.Classes;
using NaMidia.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NaMidia.UI
{
    public partial class PerfilUI : UserControl, INotifyPropertyChanged
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

        List<PERFIL> listaPerfil;
        public List<PERFIL> ListaPerfil
        {
            get { return listaPerfil; }
            set
            {
                listaPerfil = value;
                OnPropertyChanged("ListaPerfil");
            }
        }

        List<TreeViewModulo> listaPerfilModuloAcao;
        public List<TreeViewModulo> ListaPerfilModuloAcao
        {
            get { return listaPerfilModuloAcao; }
            set
            {
                listaPerfilModuloAcao = value;
                OnPropertyChanged("ListaPerfilModuloAcao");
            }
        }

        PERFIL perfil;
        public PERFIL Perfil
        {
            get { return perfil; }
            set
            {
                perfil = value;
                OnPropertyChanged("Perfil");
            }
        }
        #endregion

        #region [ Construtor ]

        public PerfilUI()
        {
            this.ListaPerfil = PerfilDAL.RecuperarListaPerfil();
            this.LinhasGridView = 20;
            this.ListaPerfilModuloAcao = new List<TreeViewModulo>();

            InitializeComponent();
            Loaded += frmCadastroPerfil_Loaded;

            VerificarOperacao(EnumTipoOperacao.Aguardar);
        }

        #endregion

        # region [ Eventos ]

        private void frmCadastroPerfil_Loaded(object sender, RoutedEventArgs e)
        {
            this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtPerfil.RowHeight);
        }

        private void dtPerfil_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                Perfil = PerfilDAL.RecuperarPerfil((dtPerfil.SelectedItem as PERFIL).cd_Perfil);
                this.ListaPerfilModuloAcao = PerfilDAL.RecuperarListaTreeViewModulo(Perfil.cd_Perfil);
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
                    Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

                    Task.Factory.StartNew(() =>
                    {
                        PerfilDAL.AlterarPerfil(this.Perfil, this.ListaPerfilModuloAcao);
                        ListaPerfil = PerfilDAL.RecuperarListaPerfil();
                    }).ContinueWith(task =>
                    {
                        Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
                        VerificarOperacao(EnumTipoOperacao.Aguardar);
                        new MensagemUI(Mensagens.registroSalvoComSucesso).Show();

                    }, TaskScheduler.FromCurrentSynchronizationContext());
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
                if (this.Perfil.cd_Perfil != 0)
                {
                    if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        PerfilDAL.ExcluirPerfil(this.Perfil.cd_Perfil);
                        ListaPerfil = PerfilDAL.RecuperarListaPerfil();
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

                    dtPerfil.IsEnabled = false;

                    radTreeListView.IsEnabled = true;

                    this.Perfil = new PERFIL();
                    this.ListaPerfilModuloAcao = PerfilDAL.RecuperarListaTreeViewModulo(null);

                    break;

                case EnumTipoOperacao.Editar:
                    txtNome.IsEnabled = true;

                    btnNovo.IsEnabled = false;
                    btnExcluir.IsEnabled = false;

                    btnSalvar.IsEnabled = true;
                    btnCancelar.IsEnabled = true;

                    radTreeListView.IsEnabled = true;
                    dtPerfil.IsEnabled = true;
                    break;

                case EnumTipoOperacao.Cancelar:
                    txtNome.IsEnabled = false;
                    txtNome.Clear();
                    txtNome.LimpaErrosValidacao();

                    btnNovo.IsEnabled = true;
                    btnEditar.IsEnabled = true;
                    btnExcluir.IsEnabled = true;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    radTreeListView.IsEnabled = false;
                    dtPerfil.IsEnabled = true;

                    if (dtPerfil.Items.Count > 0)
                        dtPerfil.SelectedItem = dtPerfil.Items[0];

                    txtNome.LimpaErrosValidacao();

                    break;

                case EnumTipoOperacao.Aguardar:
                    txtNome.Clear();
                    txtNome.IsEnabled = false;

                    btnNovo.IsEnabled = true;
                    btnEditar.IsEnabled = true;
                    btnExcluir.IsEnabled = true;

                    btnSalvar.IsEnabled = false;
                    btnCancelar.IsEnabled = false;

                    radTreeListView.IsEnabled = false;

                    dtPerfil.IsEnabled = true;

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
