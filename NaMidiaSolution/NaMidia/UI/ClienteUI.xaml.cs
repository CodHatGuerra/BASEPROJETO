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
    public partial class ClienteUI : UserControl, INotifyPropertyChanged
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
                OnPropertyChanged("LinhasGridView");
            }
        }

        PESSOA pessoa;
        public PESSOA Pessoa
        {
            get { return pessoa; }
            set
            {
                pessoa = value;
                this.OnPropertyChanged("Pessoa");
            }
        }

        private List<TIPOCONTATO> listaTipoContato;
        public List<TIPOCONTATO> ListaTipoContato
        {
            get { return this.listaTipoContato; }
            set
            {
                this.listaTipoContato = value;
                this.OnPropertyChanged("ListaTipoContato");
            }
        }

        private List<CIDADE> listaCidade;
        public List<CIDADE> ListaCidade
        {
            get { return this.listaCidade; }
            set
            {
                this.listaCidade = value;
                this.OnPropertyChanged("ListaCidade");
            }
        }

        private List<PESSOA> listaPessoas;
        public List<PESSOA> ListaPessoas
        {
            get { return this.listaPessoas; }
            set
            {
                this.listaPessoas = value;
                this.OnPropertyChanged("ListaPessoas");
            }
        }

        private const int CODIGO_COMBO_OPCAO_PADRAO = -1;

        private enum documentos { CPF, CNPJ }

        #endregion

        #region [ Construtor ]

        public ClienteUI()
        {
            this.ListaTipoContato = new List<TIPOCONTATO>();
            this.ListaCidade = new List<CIDADE>();
            this.ListaPessoas = new List<PESSOA>();
            this.LinhasGridView = 20;

            InitializeComponent();
            Loaded += ClienteUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void ClienteUI_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                this.ListaTipoContato = PessoaDAL.RecuperarListaContato();
                this.ListaCidade = CidadeDAL.RecuperarListaCidade();
                this.ListaPessoas = PessoaDAL.RecuperarListaPessoa();

            }).ContinueWith(task =>
            {
                this.LinhasGridView = (double)((this.rGridView.ActualHeight - 100) / this.dtCliente.RowHeight);
                VerificarOperacao(EnumTipoOperacao.Aguardar);
                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region [checkBox]

        private void ckbValidaCPF_Checked(object sender, RoutedEventArgs e)
        {
            this.txtCpf.LimpaErrosValidacao();
        }

        private void ckbValidaCNPJ_Checked(object sender, RoutedEventArgs e)
        {
            this.txtCnpj.LimpaErrosValidacao();
        }

        private void ckbValidaCPF_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCpf.Value != null)
                {
                    if (!this.ValidarDocumentos(documentos.CPF, this.txtCpf.Value))
                    {
                        this.txtCpf.SetaValidacao(string.Empty);
                        this.txtCpf.RaiseErroValidacao();
                    }

                    else
                        this.txtCpf.LimpaErrosValidacao();
                }

                else
                {
                    this.txtCpf.SetaValidacao(string.Empty);
                    this.txtCpf.RaiseErroValidacao();
                }
            }
            catch { }
        }

        private void ckbValidaCNPJ_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCnpj.Value != null)
                {
                    if (!this.ValidarDocumentos(documentos.CNPJ, this.txtCnpj.Value))
                    {
                        this.txtCnpj.SetaValidacao(string.Empty);
                        this.txtCnpj.RaiseErroValidacao();
                    }

                    else
                        this.txtCnpj.LimpaErrosValidacao();
                }

                else
                {
                    this.txtCnpj.SetaValidacao(string.Empty);
                    this.txtCnpj.RaiseErroValidacao();
                }
            }
            catch { }
        }

        #endregion

        #region [combobox]

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
            }
        }

        private void cbCidade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbCidade.SelectedValue != null)
                {
                    this.Pessoa.CIDADE = CidadeDAL.RecuperarCidade(Convert.ToInt32(cbCidade.SelectedValue));
                    this.Pessoa.cd_Cidade = Convert.ToInt32(cbCidade.SelectedValue);
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region [textbox]

        private void txtCpf_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ckbValidaCPF.IsChecked == false)
                {
                    if (this.txtCpf.Value != null)
                    {
                        if (!this.ValidarDocumentos(documentos.CPF, this.txtCpf.Value))
                        {
                            this.txtCpf.SetaValidacao(string.Empty);
                            this.txtCpf.RaiseErroValidacao();
                        }
                        else
                            this.txtCpf.LimpaErrosValidacao();
                    }
                    else
                    {
                        this.txtCpf.SetaValidacao(string.Empty);
                        this.txtCpf.RaiseErroValidacao();
                    }
                }
            }
            catch { }
        }

        private void txtCnpj_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ckbValidaCNPJ.IsChecked == false)
                {
                    if (this.txtCnpj.Value != null)
                    {
                        if (!this.ValidarDocumentos(documentos.CNPJ, this.txtCnpj.Value))
                        {
                            this.txtCnpj.SetaValidacao(string.Empty);
                            this.txtCnpj.RaiseErroValidacao();
                        }
                        else
                            this.txtCnpj.LimpaErrosValidacao();
                    }
                    else
                    {
                        this.txtCnpj.SetaValidacao(string.Empty);
                        this.txtCnpj.RaiseErroValidacao();
                    }
                }
            }
            catch { }
        }

        #endregion

        #region [RadGridView]

        private void dtCliente_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtCliente.SelectedItem != null)
                {
                    Pessoa = PessoaDAL.RecuperarPessoa((dtCliente.SelectedItem as PESSOA).cd_Pessoa);
                    cbCidade.SelectedValue = Pessoa.cd_Cidade;

                    this.btnExcluir.IsEnabled = Gerenciador.PermiteExcluir;

                    List<CONTATO> listaContatoCliente = PessoaDAL.RecuperarListaContatoPessoa(Pessoa.cd_Pessoa);
                    LimparCamposContato();

                    switch (listaContatoCliente.Count)
                    {
                        case 1:
                            ckbContato1.IsChecked = true;
                            txtContato1.Text = listaContatoCliente[0].ds_Contato;
                            cbContato1.SelectedValue = listaContatoCliente[0].cd_TipoContato;
                            break;

                        case 2:
                            ckbContato1.IsChecked = true;
                            txtContato1.Text = listaContatoCliente[0].ds_Contato;
                            cbContato1.SelectedValue = listaContatoCliente[0].cd_TipoContato;

                            ckbContato2.IsChecked = true;
                            txtContato2.Text = listaContatoCliente[1].ds_Contato;
                            cbContato2.SelectedValue = listaContatoCliente[1].cd_TipoContato;
                            break;

                        case 3:
                            ckbContato1.IsChecked = true;
                            txtContato1.Text = listaContatoCliente[0].ds_Contato;
                            cbContato1.SelectedValue = listaContatoCliente[0].cd_TipoContato;

                            ckbContato2.IsChecked = true;
                            txtContato2.Text = listaContatoCliente[1].ds_Contato;
                            cbContato2.SelectedValue = listaContatoCliente[1].cd_TipoContato;

                            ckbContato3.IsChecked = true;
                            txtContato3.Text = listaContatoCliente[2].ds_Contato;
                            cbContato3.SelectedValue = listaContatoCliente[2].cd_TipoContato;
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

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
                List<CONTATO> listaContato = new List<CONTATO>();
                if (ValidarCampos())
                {
                    if (ckbContato1.IsChecked == true)
                    {
                        listaContato.Add(new CONTATO()
                        {
                            cd_Pessoa = Pessoa.cd_Pessoa,
                            cd_TipoContato = Convert.ToInt16(cbContato1.SelectedValue),
                            ds_Contato = txtContato1.Text
                        });
                    }

                    if (ckbContato2.IsChecked == true)
                    {
                        listaContato.Add(new CONTATO()
                        {
                            cd_Pessoa = Pessoa.cd_Pessoa,
                            cd_TipoContato = Convert.ToInt16(cbContato2.SelectedValue),
                            ds_Contato = txtContato2.Text
                        });
                    }

                    if (ckbContato3.IsChecked == true)
                    {
                        listaContato.Add(new CONTATO()
                        {
                            cd_Pessoa = Pessoa.cd_Pessoa,
                            cd_TipoContato = Convert.ToInt16(cbContato3.SelectedValue),
                            ds_Contato = txtContato3.Text
                        });
                    }

                    PessoaDAL.AlterarPessoa(Pessoa, listaContato);
                    ListaPessoas = PessoaDAL.RecuperarListaPessoa();

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
                if (Pessoa.cd_Pessoa != 0)
                {
                    if (MessageBox.Show(Mensagens.desejaExcluirRegistro, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        PessoaDAL.ExcluirPessoa((int)Pessoa.cd_Pessoa);

                        //Atualizando a Lista
                        ListaPessoas = PessoaDAL.RecuperarListaPessoa();
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

        private bool ValidarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(txtFantasia.Text))
            {
                txtFantasia.SetaValidacao("CAMPO_OBRIGATORIO");
                txtFantasia.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtFantasia.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(txtNome.Text))
            {
                txtNome.SetaValidacao("CAMPO_OBRIGATORIO");
                txtNome.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                txtNome.LimpaErrosValidacao();


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

            if (this.ckbValidaCPF.IsChecked == false)
            {
                if (this.txtCpf.Value != null)
                {
                    if (!this.ValidarDocumentos(documentos.CPF, this.txtCpf.Value))
                    {
                        this.txtCpf.SetaValidacao(string.Empty);
                        this.txtCpf.RaiseErroValidacao();
                        formularioValidado = false;
                    }
                    else
                        this.txtCpf.LimpaErrosValidacao();
                }
                else
                {
                    this.txtCpf.SetaValidacao(string.Empty);
                    this.txtCpf.RaiseErroValidacao();
                    formularioValidado = false;
                }
            }

            if (this.ckbValidaCNPJ.IsChecked == false)
            {
                if (this.txtCnpj.Value != null)
                {
                    if (!this.ValidarDocumentos(documentos.CNPJ, this.txtCnpj.Value))
                    {
                        this.txtCnpj.SetaValidacao(string.Empty);
                        this.txtCnpj.RaiseErroValidacao();
                        formularioValidado = false;
                    }
                    else
                        this.txtCnpj.LimpaErrosValidacao();
                }
                else
                {
                    this.txtCnpj.SetaValidacao(string.Empty);
                    this.txtCnpj.RaiseErroValidacao();
                    formularioValidado = false;
                }
            }

            return formularioValidado;
        }

        private bool ValidarDocumentos(documentos documento, string valor)
        {
            switch (documento)
            {
                case documentos.CPF:
                    return ExtensaoDeMetodos.ValidaCPF(valor);

                case documentos.CNPJ:
                    return ExtensaoDeMetodos.ValidaCnpj(valor);

                default: return false;
            }
        }

        private void LimparErrosDeValidacao()
        {
            this.txtNome.LimpaErrosValidacao();
            this.txtFantasia.LimpaErrosValidacao();
            this.txtCpf.LimpaErrosValidacao();
            this.txtCnpj.LimpaErrosValidacao();
            this.cbCidade.LimpaErrosValidacao();
        }

        private void LimparCamposContato()
        {
            cbContato1.Text = string.Empty;
            cbContato2.Text = string.Empty;
            cbContato3.Text = string.Empty;

            txtContato1.Clear();
            txtContato2.Clear();
            txtContato3.Clear();

            ckbContato1.IsChecked = false;
            ckbContato2.IsChecked = false;
            ckbContato3.IsChecked = false;
        }

        private void VerificarOperacao(EnumTipoOperacao Tipo)
        {
            switch (Tipo)
            {
                #region Novo

                case EnumTipoOperacao.Novo:
                    this.txtContato1.Clear();
                    this.txtContato2.Clear();
                    this.txtContato3.Clear();
                    this.cbContato1.Text = string.Empty;
                    this.cbContato2.Text = string.Empty;
                    this.cbContato3.Text = string.Empty;
                    this.ckbContato1.IsEnabled = true;
                    this.ckbContato2.IsEnabled = true;
                    this.ckbContato3.IsEnabled = true;
                    this.ckbValidaCPF.IsChecked = false;
                    this.cbCidade.Text = string.Empty;

                    this.txtNome.IsEnabled = true;
                    this.txtBairro.IsEnabled = true;
                    this.txtCnpj.IsEnabled = true;
                    this.txtCpf.IsEnabled = true;
                    this.txtEndereco.IsEnabled = true;
                    this.txtFantasia.IsEnabled = true;
                    this.txtNome.IsEnabled = true;
                    this.txtNumero.IsEnabled = true;
                    this.txtRg.IsEnabled = true;
                    this.cbCidade.IsEnabled = true;
                    this.ckbValidaCNPJ.IsEnabled = true;
                    this.ckbValidaCPF.IsEnabled = true;
                    this.txtRazaoSocial.IsEnabled = true;

                    this.btnNovo.IsEnabled = true;
                    this.btnEditar.IsEnabled = false;
                    this.btnExcluir.IsEnabled = false;

                    this.btnSalvar.IsEnabled = true;
                    this.btnCancelar.IsEnabled = true;

                    this.dtCliente.IsEnabled = false;

                    this.Pessoa = new PESSOA();
                    this.LimparCamposContato();
                    this.LimparErrosDeValidacao();

                    break;

                #endregion Novo

                #region Editar

                case EnumTipoOperacao.Editar:
                    this.txtNome.IsEnabled = true;
                    this.txtBairro.IsEnabled = true;
                    this.txtCnpj.IsEnabled = true;
                    this.txtCpf.IsEnabled = true;
                    this.txtEndereco.IsEnabled = true;
                    this.txtFantasia.IsEnabled = true;
                    this.txtNome.IsEnabled = true;
                    this.txtNumero.IsEnabled = true;
                    this.txtRg.IsEnabled = true;
                    this.cbCidade.IsEnabled = true;
                    this.ckbValidaCNPJ.IsEnabled = true;
                    this.ckbValidaCPF.IsEnabled = true;
                    this.txtRazaoSocial.IsEnabled = true;

                    this.btnNovo.IsEnabled = false;
                    this.btnEditar.IsEnabled = true;
                    this.btnExcluir.IsEnabled = false;

                    this.btnSalvar.IsEnabled = true;
                    this.btnCancelar.IsEnabled = true;

                    this.dtCliente.IsEnabled = true;

                    this.ckbContato1.IsEnabled = true;
                    this.ckbContato2.IsEnabled = true;
                    this.ckbContato3.IsEnabled = true;

                    this.LimparErrosDeValidacao();
                    break;

                #endregion Editar

                #region Cancelar

                case EnumTipoOperacao.Cancelar:
                    this.ckbValidaCNPJ.IsChecked = false;
                    this.ckbValidaCPF.IsChecked = false;

                    this.txtNome.IsEnabled = false;
                    this.txtBairro.IsEnabled = false;
                    this.txtCnpj.IsEnabled = false;
                    this.txtCpf.IsEnabled = false;
                    this.txtEndereco.IsEnabled = false;
                    this.txtFantasia.IsEnabled = false;
                    this.txtNome.IsEnabled = false;
                    this.txtNumero.IsEnabled = false;
                    this.txtRg.IsEnabled = false;
                    this.cbCidade.IsEnabled = false;
                    this.ckbValidaCNPJ.IsEnabled = false;
                    this.cbCidade.Text = string.Empty;

                    this.ckbContato1.IsEnabled = false;
                    this.ckbContato2.IsEnabled = false;
                    this.ckbContato3.IsEnabled = false;
                    this.cbContato1.Text = string.Empty;
                    this.cbContato2.Text = string.Empty;
                    this.cbContato3.Text = string.Empty;
                    this.ckbContato1.IsChecked = false;
                    this.ckbContato2.IsChecked = false;
                    this.ckbContato3.IsChecked = false;

                    this.ckbValidaCPF.IsEnabled = false;
                    this.txtRazaoSocial.IsEnabled = false;

                    this.btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    this.btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    this.btnExcluir.IsEnabled = false;

                    this.btnSalvar.IsEnabled = false;
                    this.btnCancelar.IsEnabled = false;

                    this.dtCliente.IsEnabled = true;

                    this.Pessoa = new PESSOA();
                    this.LimparCamposContato();
                    this.LimparErrosDeValidacao();

                    break;

                #endregion Cancelar

                #region Aguardar

                case EnumTipoOperacao.Aguardar:
                    this.ckbValidaCNPJ.IsChecked = false;
                    this.ckbValidaCPF.IsChecked = false;

                    this.txtNome.IsEnabled = false;
                    this.txtBairro.IsEnabled = false;
                    this.txtCnpj.IsEnabled = false;
                    this.txtCpf.IsEnabled = false;
                    this.txtEndereco.IsEnabled = false;
                    this.txtFantasia.IsEnabled = false;
                    this.txtNome.IsEnabled = false;
                    this.txtNumero.IsEnabled = false;
                    this.txtRg.IsEnabled = false;
                    this.cbCidade.IsEnabled = false;
                    this.ckbValidaCNPJ.IsEnabled = false;
                    this.cbCidade.Text = string.Empty;
                    this.ckbContato1.IsChecked = false;
                    this.ckbContato2.IsChecked = false;
                    this.ckbContato3.IsChecked = false;
                    this.cbContato1.Text = string.Empty;
                    this.cbContato2.Text = string.Empty;
                    this.cbContato3.Text = string.Empty;
                    this.ckbContato1.IsEnabled = false;
                    this.ckbContato2.IsEnabled = false;
                    this.ckbContato3.IsEnabled = false;

                    this.ckbValidaCPF.IsEnabled = false;
                    this.txtRazaoSocial.IsEnabled = false;

                    this.btnNovo.IsEnabled = Gerenciador.PermiteInserir;
                    this.btnEditar.IsEnabled = Gerenciador.PermiteEditar;
                    this.btnExcluir.IsEnabled = false;

                    this.btnSalvar.IsEnabled = false;
                    this.btnCancelar.IsEnabled = false;

                    this.dtCliente.IsEnabled = true;

                    this.Pessoa = new PESSOA();
                    this.LimparCamposContato();
                    this.LimparErrosDeValidacao();

                    break;
            }

            #endregion Aguardar
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
