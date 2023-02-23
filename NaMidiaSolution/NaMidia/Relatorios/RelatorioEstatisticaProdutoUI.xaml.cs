using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using RadGridViewPrint;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using System.Threading.Tasks;

namespace NaMidia.Relatorios
{
    public partial class RelatorioEstatisticaProdutoUI : UserControl, INotifyPropertyChanged
    {
        #region [Propriedades]

        public event PropertyChangedEventHandler PropertyChanged;

        private enum EnumTipoRelatorio { anual = 0, mensal = 1 }
        EnumTipoRelatorio enumTipo;

        List<EstatisticaProduto> listaEstatisticaProduto;
        public List<EstatisticaProduto> ListaEstatisticaProduto
        {
            get { return listaEstatisticaProduto; }
            set
            {
                listaEstatisticaProduto = value;
                this.OnPropertyChanged("ListaEstatisticaProduto");
            }
        }

        List<EstatisticaProduto> listaEstatisticaProdutoAux;
        public List<EstatisticaProduto> ListaEstatisticaProdutoAux
        {
            get { return listaEstatisticaProdutoAux; }
            set
            {
                listaEstatisticaProdutoAux = value;
                this.OnPropertyChanged("ListaEstatisticaProdutoAux");
            }
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        int ano, mes;
        #endregion

        #region [ Construtor ]

        public RelatorioEstatisticaProdutoUI()
        {
            this.ListaEstatisticaProduto = new List<EstatisticaProduto>();
            this.ListaEstatisticaProdutoAux = new List<EstatisticaProduto>();

            InitializeComponent();
        }

        #endregion

        #region [ Eventos ]

        private void btnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new PrintExportExtensions().ExportRadGridViewToExcel(dtProdutos))
                    MessageBox.Show(Mensagens.graficoExporadoComSucesso, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportarPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new PrintExportExtensions().ExportRadGridViewToPDF(dtProdutos))
                    MessageBox.Show(Mensagens.graficoExporadoComSucesso, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbTipoRelatorio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                enumTipo = cbTipoRelatorio.SelectedIndex == 0 ? EnumTipoRelatorio.anual : EnumTipoRelatorio.mensal;

                if (enumTipo == EnumTipoRelatorio.anual)
                {
                    CultureInfo cultureInfo = new CultureInfo("pt-BR");
                    DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
                    dateInfo.ShortDatePattern = "yyyy";
                    cultureInfo.DateTimeFormat = dateInfo;
                    dpData.Culture = cultureInfo;

                    dpData.DateSelectionMode = Telerik.Windows.Controls.Calendar.DateSelectionMode.Year;
                }

                else if (enumTipo == EnumTipoRelatorio.mensal)
                {
                    CultureInfo cultureInfo = new CultureInfo("pt-BR");
                    DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
                    dateInfo.ShortDatePattern = "MM/yyyy";
                    cultureInfo.DateTimeFormat = dateInfo;
                    dpData.Culture = cultureInfo;

                    dpData.DateSelectionMode = Telerik.Windows.Controls.Calendar.DateSelectionMode.Month;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpData.SelectedDate == null)
                    MessageBox.Show(Mensagens.selecioneUmaData);

                else
                {
                    Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;
                    this.ano = dpData.DisplayDate.Year;
                    this.mes = dpData.DisplayDate.Month;

                    Task.Factory.StartNew(() => CarregarRegistros(ano, mes)).ContinueWith(task =>
                    {
                        Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbTipoRelatorio_Loaded(object sender, RoutedEventArgs e)
        {
            RadComboBox combo = (RadComboBox)sender;
            combo.SelectedIndex = 0;
        }
        #endregion

        #region [ Métodos ]

        private void CarregarRegistros(int? ano, int? mes)
        {
            if (this.ListaEstatisticaProduto != null)
                this.ListaEstatisticaProduto.Clear();

            if (ListaEstatisticaProdutoAux != null)
                this.ListaEstatisticaProdutoAux.Clear();

            foreach (var item in ProdutoDAL.RecuperarListaProdutos())
            {
                if (enumTipo == EnumTipoRelatorio.anual)
                {
                    foreach (var produtomalha in GraficoDAL.QuantidadeProdutosVendidosPorMes(item.cd_Produto, Convert.ToInt32(ano)))
                    {
                        EstatisticaProduto estatisticaProduto = new EstatisticaProduto();
                        estatisticaProduto.QuantidadeProduto = produtomalha.Item1;
                        estatisticaProduto.DescricaoProduto = item.ds_Produto;
                        estatisticaProduto.DescricaoMalha = produtomalha.Item2;
                        ListaEstatisticaProdutoAux.Add(estatisticaProduto);
                    }
                }

                if (enumTipo == EnumTipoRelatorio.mensal)
                {
                    foreach (var produtomalha in GraficoDAL.QuantidadeProdutosVendidosPorMes(item.cd_Produto, Convert.ToInt32(ano), Convert.ToInt32(mes)))
                    {
                        EstatisticaProduto estatisticaProduto = new EstatisticaProduto();
                        estatisticaProduto.QuantidadeProduto = produtomalha.Item1;
                        estatisticaProduto.DescricaoProduto = item.ds_Produto;
                        estatisticaProduto.DescricaoMalha = produtomalha.Item2;
                        ListaEstatisticaProdutoAux.Add(estatisticaProduto);
                    }
                }

                ListaEstatisticaProduto = ListaEstatisticaProdutoAux.OrderByDescending(a => a.QuantidadeProduto).ToList();
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

    public class EstatisticaProduto
    {
        int quantidadeProduto;
        public int QuantidadeProduto
        {
            get { return quantidadeProduto; }
            set { quantidadeProduto = value; }
        }

        string descricaoProduto;
        public string DescricaoProduto
        {
            get { return descricaoProduto; }
            set { descricaoProduto = value; }
        }

        string descricaoMalha;
        public string DescricaoMalha
        {
            get { return descricaoMalha; }
            set { descricaoMalha = value; }
        }
    }
}
