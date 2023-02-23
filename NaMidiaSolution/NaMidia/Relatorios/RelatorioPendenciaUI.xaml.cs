using NaMidiaCore.ClassesDAL;
using System;
using System.Windows;
using System.Windows.Controls;
using NaMidia.Classes;
using System.ComponentModel;
using Telerik.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;

namespace NaMidia.RELATORIOS
{
    public partial class RelPendenciaUI : UserControl, INotifyPropertyChanged
    {
        #region [Propriedades]

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public event PropertyChangedEventHandler PropertyChanged;

        private enum tipoPesquisa { pendente, quitado };
        tipoPesquisa enumTipoPesquisa;

        decimal? valorTotal;
        public decimal? ValorTotal
        {
            get { return valorTotal; }
            set
            {
                valorTotal = value;
                OnPropertyChanged("ValorTotal");
            }
        }

        decimal? valorRecebido;
        public decimal? ValorRecebido
        {
            get { return valorRecebido; }
            set
            {
                valorRecebido = value;
                OnPropertyChanged("ValorRecebido");
            }
        }

        decimal? valorRestante;
        public decimal? ValorRestante
        {
            get { return valorRestante; }
            set
            {
                valorRestante = value;
                OnPropertyChanged("ValorRestante");
            }
        }

        DateTime dataInicial;
        DateTime dataFinal;

        List<PagamentoPendencia> listaPagamentoPendencia;

        public List<PagamentoPendencia> ListaPagamentoPendencia
        {
            get { return listaPagamentoPendencia; }
            set
            {
                listaPagamentoPendencia = value;
                OnPropertyChanged("ListaPagamentoPendencia");
            }
        }

        #endregion

        #region [Construtor]

        public RelPendenciaUI()
        {
            this.ValorTotal = 0;
            this.ValorRecebido = 0;
            this.ValorRestante = 0;
            this.ListaPagamentoPendencia = new List<PagamentoPendencia>();

            InitializeComponent();
            Loaded += RelPendenciaUI_Loaded;
        }

        private void RelPendenciaUI_Loaded(object sender, RoutedEventArgs e)
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        #endregion

        #region [Eventos]

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    dataInicial = dtInicial.SelectedDate.Value;
                    dataFinal = dtFinal.SelectedDate.Value;

                    if (rdbPendente.IsChecked == true)
                    {
                        enumTipoPesquisa = tipoPesquisa.pendente;

                        dtPendencias.Columns[4].Header = "Data Vencimento";
                        dtPendencias.Columns[3].IsVisible = false;
                    }

                    else if (rdbQuitado.IsChecked == true)
                    {
                        enumTipoPesquisa = tipoPesquisa.quitado;

                        dtPendencias.Columns[4].Header = "Data Pagamento";
                        dtPendencias.Columns[3].IsVisible = true;
                    }

                    this.rbAguarde.IsBusy = true;
                    this.worker.RunWorkerAsync();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.CalcularValores();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.rbAguarde.IsBusy = false;
            }));

        }

        #endregion

        #region [Metodos]

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

        public bool ValidarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(dtInicial.Text))
            {
                dtInicial.SetaValidacao("CAMPO_OBRIGATORIO");
                dtInicial.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                dtInicial.LimpaErrosValidacao();

            if (string.IsNullOrEmpty(dtFinal.Text))
            {
                dtFinal.SetaValidacao("CAMPO_OBRIGATORIO");
                dtFinal.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                dtFinal.LimpaErrosValidacao();

            if (rdbPendente.IsChecked == false && rdbQuitado.IsChecked == false)
            {
                rdbPendente.SetaValidacao("CAMPO_OBRIGATORIO");
                rdbPendente.RaiseErroValidacao();
                rdbQuitado.SetaValidacao("CAMPO_OBRIGATORIO");
                rdbQuitado.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
            {
                rdbPendente.LimpaErrosValidacao();
                rdbQuitado.LimpaErrosValidacao();
            }

            return formularioValidado;
        }

        private void CalcularValores()
        {
            if (ListaPagamentoPendencia.Count > 0)
                ListaPagamentoPendencia.Clear();

            PendenciaDAL.CalcularValorTotalRecebido(dataInicial, dataFinal);

            ListaPagamentoPendencia = enumTipoPesquisa == tipoPesquisa.pendente ?
                 PendenciaDAL.ConsultarPendencias(dataInicial, dataFinal) :
                 PendenciaDAL.ConsultarRecebido(dataInicial, dataFinal);

            this.ValorTotal = PendenciaDAL.valorTotal;
            this.ValorRecebido = PendenciaDAL.valorRecebido;
            this.ValorRestante = (PendenciaDAL.valorTotal - PendenciaDAL.valorRecebido);
        }

        #endregion
    }
}
