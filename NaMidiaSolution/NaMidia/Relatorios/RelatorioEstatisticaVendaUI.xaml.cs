using Microsoft.Win32;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using RadGridViewPrint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Model;

namespace NaMidia.RELATORIOS
{
    public partial class RelatorioEstatisticaVendaUI : UserControl
    {
        #region [ Construtor ]

        public RelatorioEstatisticaVendaUI()
        {
            InitializeComponent();
            Loaded += RelatorioEstatisticaVendaUI_Loaded;

            RadChartEstatisticas.DefaultView.ChartArea.LabelFormatBehavior = LabelFormatBehavior.None;
        }

        #endregion

        #region  [ Propriedades ]

        RadDocument document = new RadDocument();

        #endregion

        #region [ Eventos ]

        private void RelatorioEstatisticaVendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            for (int year = 2000; year <= DateTime.UtcNow.Year; ++year)
                cbAno.Items.Add(year);

            cbAno.Text = DateTime.UtcNow.Year.ToString();
            cbTipoDados.SelectedIndex = 0;
        }

        private void cbTipoDados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (validarCampos())
                {
                    if (cbTipoDados.SelectedIndex == 0) //Quantidade de Vendas
                        QuantidadeVendasAnual(Convert.ToInt16(cbAno.Text));

                    else if (cbTipoDados.SelectedIndex == 1) // Valor das Vendas
                        ValorVendasAnual(Convert.ToInt16(cbAno.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbAno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cbTipoDados.Text = string.Empty;
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
                if (new PrintExportExtensions().ExportRadChartToPDF(RadChartEstatisticas))
                    MessageBox.Show(Mensagens.graficoExporadoComSucesso, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void btnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new PrintExportExtensions().ExportRadChartToExcel(RadChartEstatisticas))
                    MessageBox.Show(Mensagens.graficoExporadoComSucesso, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        #endregion

        #region [ Metodos ]

        public void ValorVendasAnual(int ano)
        {
            List<decimal> lista = new List<decimal>();

            foreach (var item in GraficoDAL.ValorDeVendasPorMes(ano))
            {
                if (item == null)
                    lista.Add(0);

                else
                    lista.Add(item.Value);
            }

            decimal?[] ValorVendasAnual = new decimal?[] { lista[0], 
                                                           lista[1], 
                                                           lista[2], 
                                                           lista[3], 
                                                           lista[4], 
                                                           lista[5], 
                                                           lista[6], 
                                                           lista[7], 
                                                           lista[8], 
                                                           lista[9], 
                                                           lista[10],
                                                           lista[11]};

            this.RadChartEstatisticas.PaletteBrushesUseSolidColors = true;
            this.RadChartEstatisticas.DefaultSeriesDefinition.LegendDisplayMode = LegendDisplayMode.DataPointLabel;
            this.RadChartEstatisticas.ItemsSource = ValorVendasAnual;
        }

        public void QuantidadeVendasAnual(int ano)
        {
            List<int> lista = GraficoDAL.QuantidadeDeVendasPorMes(ano);
            double[] QuantidadeVendasAnual = new double[] { lista[0], 
                                                            lista[1], 
                                                            lista[2], 
                                                            lista[3], 
                                                            lista[4], 
                                                            lista[5], 
                                                            lista[6], 
                                                            lista[7], 
                                                            lista[8], 
                                                            lista[9], 
                                                            lista[10],
                                                            lista[11]};

            this.RadChartEstatisticas.PaletteBrushesUseSolidColors = true;
            this.RadChartEstatisticas.DefaultSeriesDefinition.LegendDisplayMode = LegendDisplayMode.DataPointLabel;
            this.RadChartEstatisticas.ItemsSource = QuantidadeVendasAnual;
        }

        public bool validarCampos()
        {
            bool formularioValidado = true;

            if (string.IsNullOrEmpty(cbAno.Text))
            {
                cbAno.SetaValidacao("CAMPO_OBRIGATORIO");
                cbAno.RaiseErroValidacao();
                formularioValidado = false;
            }

            else
                cbAno.LimpaErrosValidacao();
            return formularioValidado;
        }

        public RadDocument CreateDocument()
        {
            Section section = new Section();
            Paragraph paragraph = new Paragraph();

            MemoryStream ms = new MemoryStream();
            RadChartEstatisticas.ExportToImage(ms, new PngBitmapEncoder());
            ImageInline image = new ImageInline(ms, new Size(700, 380), "png");

            paragraph.Inlines.Add(image);
            section.Blocks.Add(paragraph);
            document.Sections.Add(section);

            ms.Close();

            return document;
        }

        #endregion
    }
}
