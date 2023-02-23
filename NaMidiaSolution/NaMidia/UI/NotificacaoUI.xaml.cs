using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using RadGridViewPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace NaMidia.UI
{
    public partial class NotificacaoUI : RadWindow, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        string tabItemAtivo;

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

        List<VENDA> listaVenda;
        public List<VENDA> ListaVenda
        {
            get { return listaVenda; }
            set
            {
                listaVenda = value;
                OnPropertyChanged("ListaVenda");
            }
        }

        List<PAGAMENTOVENDA> listaPagamento;
        public List<PAGAMENTOVENDA> ListaPagamento
        {
            get { return listaPagamento; }
            set
            {
                listaPagamento = value;
                OnPropertyChanged("ListaPagamento");
            }
        }

        int cd_Venda = 0;
        int cd_Pagamento = 0;
        bool exibirNotificacoesOcultas = false;

        #endregion

        #region [ Construtor ]

        public NotificacaoUI()
        {
            this.ListaVenda = new List<VENDA>();
            this.ListaPagamento = new List<PAGAMENTOVENDA>();

            InitializeComponent();
            Loaded += NotificacaoUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void NotificacaoUI_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }
        }

        private void RadMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (tabItemAtivo == "tiEntregas")
                {
                    cd_Venda = ((VENDA)dtVendas.SelectedItem).cd_Venda;

                    if (cd_Venda != 0)
                    {
                        rdAguarde.IsBusy = true;
                        Task.Run(() =>
                        {
                            NotificacaoDAL.ExcluirNotificacoesVenda(false, cd_Venda);
                            ListaVenda = NotificacaoDAL.RecuperarListaVenda(exibirNotificacoesOcultas);
                        }).ContinueWith((t) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
                        });
                    }
                }

                else
                {
                    cd_Pagamento = ((PAGAMENTOVENDA)dtPagamentos.SelectedItem).cd_PagamentoVenda;

                    if (cd_Pagamento != 0)
                    {
                        rdAguarde.IsBusy = true;
                        Task.Run(() =>
                        {
                            NotificacaoDAL.ExcluirNotificacoesPagamento(false, cd_Pagamento);
                            ListaPagamento = NotificacaoDAL.RecuperarListaPagamento(exibirNotificacoesOcultas);
                        }).ContinueWith((t) =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
                        });
                    }
                }
            }
            catch
            {
                MessageBox.Show(Mensagens.selecioneUmRegistro);
            }
        }

        private void tcNotificacao_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            RadSelectionChangedEventArgs selectionArgs = (RadSelectionChangedEventArgs)e;
            tabItemAtivo = ((RadTabItem)selectionArgs.AddedItems[0]).Name.ToString();

            if (tabItemAtivo == "tiEntregas")
            {
                rdAguarde.IsBusy = true;

                Task.Run(() =>
                {
                    ListaVenda = NotificacaoDAL.RecuperarListaVenda(exibirNotificacoesOcultas);
                }).ContinueWith((t) =>
                {
                    Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
                });
            }

            else
            {
                rdAguarde.IsBusy = true;

                Task.Run(() =>
                {
                    ListaPagamento = NotificacaoDAL.RecuperarListaPagamento(exibirNotificacoesOcultas);
                }).ContinueWith((t) =>
                {
                    Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
                });
            }
        }

        private void btnExcluirTodos_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show(Mensagens.excluirTodosRegistros, Mensagens.tituloMessageBox, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                if (tabItemAtivo == "tiEntregas")
                {
                    rdAguarde.IsBusy = true;

                    Task.Run(() =>
                    {
                        NotificacaoDAL.ExcluirNotificacoesVenda(true, null);
                        this.ListaVenda.Clear();
                    }).ContinueWith((t) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
                    });
                }

                else
                {
                    rdAguarde.IsBusy = true;

                    Task.Run(() =>
                    {
                        NotificacaoDAL.ExcluirNotificacoesPagamento(true, null);
                        this.ListaPagamento.Clear();
                    }).ContinueWith((t) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
                    });
                }
            }
        }

        private void dtpagamentos_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var s = e.OriginalSource as FrameworkElement;

            if (s is TextBlock)
            {
                var parentRow = s.ParentOfType<GridViewRow>();

                if (parentRow != null)
                    parentRow.IsSelected = true;
            }
        }

        private void ckbExibirNotificacoes_Checked(object sender, RoutedEventArgs e)
        {
            rdAguarde.IsBusy = true;
            exibirNotificacoesOcultas = true;

            Task.Run(() =>
            {
                ListaVenda = NotificacaoDAL.RecuperarListaVenda(exibirNotificacoesOcultas);
                ListaPagamento = NotificacaoDAL.RecuperarListaPagamento(exibirNotificacoesOcultas);
            }).ContinueWith((t) =>
            {
                Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
            });
        }

        private void ckbExibirNotificacoes_Unchecked(object sender, RoutedEventArgs e)
        {
            rdAguarde.IsBusy = true;
            exibirNotificacoesOcultas = false;

            Task.Run(() =>
            {
                ListaVenda = NotificacaoDAL.RecuperarListaVenda(exibirNotificacoesOcultas);
                ListaPagamento = NotificacaoDAL.RecuperarListaPagamento(exibirNotificacoesOcultas);
            }).ContinueWith((t) =>
            {
                Dispatcher.BeginInvoke(new Action(() => { this.rdAguarde.IsBusy = false; }));
            });
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dtVendas.Columns["colunaSituacao"].IsVisible = false;
                dtPagamentos.Columns["colunaSituacao"].IsVisible = false;

                if (new PrintExportExtensions().ExportRadGridViewToExcel(tabItemAtivo == "tiEntregas" ? dtVendas : dtPagamentos))
                    new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            finally
            {
                dtVendas.Columns["colunaSituacao"].IsVisible = true;
                dtPagamentos.Columns["colunaSituacao"].IsVisible = true;
            }
        }

        #endregion

        #region [ Métodos ]

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
