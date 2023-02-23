using NaMidia.Classes;
using NaMidia.UI;
using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using RadGridViewPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace NaMidia.RELATORIOS
{
    public partial class RelatorioVendaUI : UserControl, INotifyPropertyChanged
    {
        #region [ Propriedades ]

        public event PropertyChangedEventHandler PropertyChanged;

        List<ViewRelatorioVenda> listaVenda;
        public List<ViewRelatorioVenda> ListaVenda
        {
            get { return listaVenda; }
            set
            {
                listaVenda = value;
                OnPropertyChanged("ListaVenda");
            }
        }

        byte[] dadosImagem = null;

        ViewRelatorioVenda viewRelatorioVenda;

        IMAGEMVENDA imagemVenda;

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

        IColumnFilterDescriptor FiltroCliente { get; set; }
        TextBox txtFiltroCliente { get; set; }
        #endregion

        #region [ Construtor ]

        public RelatorioVendaUI()
        {
            this.LinhasGridView = 20;

            this.ListaVenda = new List<ViewRelatorioVenda>();

            InitializeComponent();

            this.Loaded += RelatorioVendaUI_Loaded;
        }

        #endregion

        #region [ Eventos ]

        private void RelatorioVendaUI_Loaded(object sender, RoutedEventArgs e)
        {
            Gerenciador.MainWindow.RadBusyIndicator.IsBusy = true;

            Telerik.Windows.Controls.GridViewColumn ColunaClienteFiltro = this.dtVenda.Columns[1];
            FiltroCliente = ColunaClienteFiltro.ColumnFilterDescriptor;
            FiltroCliente.FieldFilter.Filter1.Operator = Telerik.Windows.Data.FilterOperator.Contains;
            FiltroCliente.FieldFilter.Filter1.IsCaseSensitive = false;

            Task.Factory.StartNew(() =>
            {
                this.ListaVenda = VendaDAL.RecuperarRelatorioVenda();

            }).ContinueWith(task =>
            {
                this.LinhasGridView = (double)((rGridView.ActualHeight - 175) / dtVenda.RowHeight);

                Gerenciador.MainWindow.RadBusyIndicator.IsBusy = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new OpcoesExportacaoUI().ShowDialog();

                if (OpcoesExportacaoUI.tipoExportacao == EnumTipoExportacao.ExportarSelecionado)
                {
                    if (viewRelatorioVenda.cd_Venda != 0)
                        new HostVenda(viewRelatorioVenda.cd_Venda).ShowDialog();

                    else
                        MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }


                else if (OpcoesExportacaoUI.tipoExportacao == EnumTipoExportacao.ExportarTodosExcel)
                {
                    if (new PrintExportExtensions().ExportRadGridViewToExcel(dtVenda))
                        new MensagemUI(Mensagens.registrosExportadoComSucesso).Show();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtVenda_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                if (dtVenda.SelectedItem != null)
                    viewRelatorioVenda = dtVenda.SelectedItem as ViewRelatorioVenda;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dtVenda_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var s = e.OriginalSource as FrameworkElement;

            if (s is TextBlock)
            {
                var parentRow = s.ParentOfType<GridViewRow>();

                if (parentRow != null)
                    parentRow.IsSelected = true;
            }
        }

        private void miSalvar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (viewRelatorioVenda != null)
                {
                    dadosImagem = null;

                    System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                    sfd.Filter = "JPEG (.jpg)|*.jpg|BMP (.bmp)|*.bmp|PNG (.png)|*.png";

                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        System.IO.File.WriteAllBytes(sfd.FileName, VendaDAL.RecuperarImagemVenda(viewRelatorioVenda.cd_Venda).ds_Imagem.ToArray());

                        new MensagemUI(Mensagens.imagemSalvoComSucesso).Show();
                        viewRelatorioVenda = null;
                    }
                }
                else
                    MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miVer_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (viewRelatorioVenda != null)
                {
                    dadosImagem = null;

                    imagemVenda = new IMAGEMVENDA();

                    RadWindow wndVerImagem = new RadWindow();
                    wndVerImagem.ResizeMode = ResizeMode.NoResize;
                    wndVerImagem.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    wndVerImagem.Header = "Imagem";
                    StyleManager.SetTheme(wndVerImagem, new VistaTheme());

                    Grid grid = new Grid();

                    Image ImagemCamiseta = new Image();
                    ImagemCamiseta.Height = 400;
                    ImagemCamiseta.Width = 161;

                    imagemVenda = VendaDAL.RecuperarImagemVenda(viewRelatorioVenda.cd_Venda);

                    dadosImagem = imagemVenda.ds_Imagem.ToArray();

                    byte[] data = dadosImagem;

                    MemoryStream strm = new MemoryStream();

                    strm.Write(data, 0, data.Length);

                    strm.Position = 0;

                    System.Drawing.Image img = System.Drawing.Image.FromStream(strm);

                    BitmapImage bi = new BitmapImage();

                    bi.BeginInit();

                    MemoryStream ms = new MemoryStream();

                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                    ms.Seek(0, SeekOrigin.Begin);

                    bi.StreamSource = ms;

                    bi.EndInit();

                    ImagemCamiseta.Source = bi;

                    grid.Children.Add(ImagemCamiseta);

                    wndVerImagem.Content = grid;

                    wndVerImagem.Show();

                    viewRelatorioVenda = null;
                }
                else
                    MessageBox.Show(Mensagens.selecioneUmRegistro, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFiltroCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var text = (sender as TextBox);

                if (txtFiltroCliente == null)
                    txtFiltroCliente = text;

                if (FiltroCliente != null)
                    FiltroCliente.FieldFilter.Filter1.Value = txtFiltroCliente.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region [ Metodos ]

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
