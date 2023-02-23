using NaMidia.Classes;
using NaMidia.UI;
using NaMidiaCore.Classes;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TransitionEffects;

namespace NaMidia
{
    public partial class MainWindow : Window
    {
        #region [ Propriedades ]

        int tipoTransicao;

        public static double tamanhoColuna;

        private bool exibirMensagem = true;

        #endregion

        #region [ Construtor ]

        public MainWindow(int tipoTransicao)
        {
            // Aplica o tema a toda aplicação
            StyleManager.ApplicationTheme = new SummerTheme();

            InitializeComponent();

            Gerenciador.MainWindow = this;

            //Traduz os componentes
            LocalizationManager.Manager = new CustomLocalizationManager();

            this.tipoTransicao = tipoTransicao;
        }

        #endregion

        #region [ Eventos ]

        private void wndPrincipal_Load(object sender, RoutedEventArgs e)
        {
            this.transicao(tipoTransicao);
            tcTransicao.Content = new InicialUI();
            lblUsuario.Content = Gerenciador.UsuarioAtivo.FUNCIONARIO.nm_Funcionario;

            CarregarMenu();
        }

        private void wndPrincipal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (exibirMensagem)
            {
                if (System.Windows.MessageBox.Show(Mensagens.desejaRealmenteSair, Mensagens.tituloMessageBox, MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
                    e.Cancel = true;
            }

            else
                exibirMensagem = true;
        }

        private void btnNotificacao_Click(object sender, RoutedEventArgs e)
        {
            NotificacaoUI notificacao = new NotificacaoUI();
            notificacao.Show();
        }

        private void RdMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var radMenuItem = e.OriginalSource as RadMenuItem;
            Navegar((ModuloEnum)radMenuItem.Tag);
        }

        #endregion

        #region [ Metodos ]

        public void CarregarMenu()
        {
            var lista = SistemaDAL.RecuperarListaModulo(Gerenciador.UsuarioAtivo.cd_Login);
            Func<MODULO, RadMenuItem> funcModulo = null;

            funcModulo = (modulo =>
            {
                RadMenuItem rdMenuItem = new RadMenuItem();

                foreach (var item in lista.Where(a => a.cd_ModuloPai == modulo.cd_Modulo).OrderBy(a => a.ds_OrdemExibicao).ToList())
                    rdMenuItem.Items.Add(funcModulo(item));

                rdMenuItem.Header = modulo.ds_Modulo;
                rdMenuItem.Tag = modulo.cd_Modulo;

                if (modulo.ds_Icon != null)
                    rdMenuItem.Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Imagens/" + modulo.ds_Icon, UriKind.RelativeOrAbsolute)),
                        Width = 12,
                        Height = 12
                    };
                return rdMenuItem;

            });

            int contador = 0;
            foreach (var item in lista.Where(a => a.cd_ModuloPai == null && a.cd_Modulo != (int)ModuloEnum.Notificacao).OrderBy(a => a.ds_OrdemExibicao).ToList())
            {
                if (contador == 0)
                {
                    mnuPrincipal.Items.Add(funcModulo(item));
                    contador++;
                }
                else
                {
                    mnuPrincipal.Items.Add(new RadMenuSeparatorItem());
                    mnuPrincipal.Items.Add(funcModulo(item));
                }
            }

            this.btnNotificacao.IsEnabled = lista.Where(a => a.cd_Modulo == (int)ModuloEnum.Notificacao).Count() > 0;
        }

        public void Navegar(ModuloEnum moduloEnum)
        {
            transicao(1);
            CarregarModuloAcao((int)moduloEnum);

            #region [ Inicio ]

            if (moduloEnum == ModuloEnum.Inicio)
                tcTransicao.Content = new InicialUI();

            #endregion

            #region [ Cadastros ]

            else if (moduloEnum == ModuloEnum.Cidade)
                tcTransicao.Content = new CidadeUI();

            else if (moduloEnum == ModuloEnum.Cliente)
                tcTransicao.Content = new ClienteUI();

            else if (moduloEnum == ModuloEnum.Costureira)
                tcTransicao.Content = new CostureiraUI();

            else if (moduloEnum == ModuloEnum.Fornecedor)
                tcTransicao.Content = new FornecedorUI();

            else if (moduloEnum == ModuloEnum.Funcionário)
                tcTransicao.Content = new FuncionarioUI();

            else if (moduloEnum == ModuloEnum.Produto)
                tcTransicao.Content = new ProdutoUI();

            else if (moduloEnum == ModuloEnum.Gola)
                tcTransicao.Content = new GolaUI();

            else if (moduloEnum == ModuloEnum.Malha)
                tcTransicao.Content = new MalhaUI();

            else if (moduloEnum == ModuloEnum.Tamanho)
                tcTransicao.Content = new TamanhoUI();

            #endregion

            #region [ Vendas ]

            else if (moduloEnum == ModuloEnum.Venda)
                tcTransicao.Content = new VendaUI();
            else if (moduloEnum == ModuloEnum.Controle_Costureira)
                tcTransicao.Content = new CostureiraPedidoUI();
            else if (moduloEnum == ModuloEnum.Controle_Entrega)
                tcTransicao.Content = new VendaEntregaUI();

            #endregion

            #region [ Pagamentos ]

            else if (moduloEnum == ModuloEnum.Controle_do_Mês)
                tcTransicao.Content = new PagamentoVendaUI();

            else if (moduloEnum == ModuloEnum.Pagamento_Costureira)
                tcTransicao.Content = new PagamentoCostureiraUI();

            #endregion

            #region [ Relatórios ]

            else if (moduloEnum == ModuloEnum.Estatísticas_de_Produtos)
                tcTransicao.Content = new NaMidia.Relatorios.RelatorioEstatisticaProdutoUI();

            else if (moduloEnum == ModuloEnum.Estatísticas_de_Vendas)
                tcTransicao.Content = new NaMidia.RELATORIOS.RelatorioEstatisticaVendaUI();

            else if (moduloEnum == ModuloEnum.Pagamentos)
                tcTransicao.Content = new NaMidia.RELATORIOS.RelatorioPagamentoVendaUI();

            else if (moduloEnum == ModuloEnum.Pendências_Pagamentos)
                tcTransicao.Content = new NaMidia.RELATORIOS.RelPendenciaUI();

            else if (moduloEnum == ModuloEnum.VendaRelatorio)
                tcTransicao.Content = new NaMidia.RELATORIOS.RelatorioVendaUI();

            else if (moduloEnum == ModuloEnum.Relatorio_Costureira)
                tcTransicao.Content = new NaMidia.RELATORIOS.RelatorioCostureiraUI();

            #endregion

            #region [ Seguranca ]

            else if (moduloEnum == ModuloEnum.Perfil)
                tcTransicao.Content = new PerfilUI();

            else if (moduloEnum == ModuloEnum.Usuário)
                tcTransicao.Content = new UsuarioUI();

            #endregion

            #region [ Opções ]

            else if (moduloEnum == ModuloEnum.Logs)
            {
                ConfiguracaoUI configuracao = new ConfiguracaoUI();
                configuracao.Show();
            }

            else if (moduloEnum == ModuloEnum.Trocar_Usuário)
            {
                exibirMensagem = false;
                LoginUI login = new LoginUI();
                login.Show();
                this.Close();
            }

            else if (moduloEnum == ModuloEnum.Sair_da_Aplicação)
                this.Close();

            #endregion

            #region [ Ajuda ]

            else if (moduloEnum == ModuloEnum.Manual)
            {
                string diretorio = Path.Combine(Environment.CurrentDirectory, "Ajuda/naMidiaAjuda.chm");
                System.Diagnostics.Process.Start(diretorio);
            }

            else if (moduloEnum == ModuloEnum.Sobre)
            {
                SobreUI sobre = new SobreUI();
                sobre.Show();
            }

            #endregion
        }

        private void CarregarModuloAcao(int codigoModulo)
        {
            Gerenciador.CodigoModuloAtivo = codigoModulo;

            if (!Gerenciador.DicModuloAcao.ContainsKey(codigoModulo))
                Gerenciador.DicModuloAcao.Add(codigoModulo, SistemaDAL.RecuperarListaModuloAcao(Gerenciador.UsuarioAtivo.cd_Login, codigoModulo).Select(a => a.cd_Acao).ToList());

            this.RecuperarAcoesModuloAtivo(codigoModulo);
        }

        public void RecuperarAcoesModuloAtivo(int codigoModulo)
        {
            List<int> lstAcoes = Gerenciador.DicModuloAcao[codigoModulo];
            Gerenciador.PermiteEditar = lstAcoes.Where(a => a == (int)AcaoEnum.Editar).Count() > 0;
            Gerenciador.PermiteInserir = lstAcoes.Where(a => a == (int)AcaoEnum.Inserir).Count() > 0;
            Gerenciador.PermiteExcluir = lstAcoes.Where(a => a == (int)AcaoEnum.Remover).Count() > 0;
        }

        public void transicao(int i)
        {
            if (i == 0)
                tcTransicao.Transition = new FadeTransition();

            else
                tcTransicao.Transition = new SlideAndZoomTransition();
        }

        #endregion
    }
}
