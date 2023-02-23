using NaMidia.Classes;
using System.Windows;
using Telerik.Windows.Controls;

namespace NaMidia.UI
{
    public partial class OpcoesExportacaoUI : RadWindow
    {
        public static EnumTipoExportacao tipoExportacao;

        public OpcoesExportacaoUI()
        {
            InitializeComponent();
            Loaded += OpcoesExportacaoUI_Loaded;
            tipoExportacao = EnumTipoExportacao.Nenhum;
        }

        private void OpcoesExportacaoUI_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.ParentOfType<Window>();

            if (window != null)
            {
                RadWindow radWindow = RadWindow.GetParentRadWindow(this);
                window.ShowInTaskbar = true;
            }
        }

        private void btnExportarTudoExcel_Click(object sender, RoutedEventArgs e)
        {
            tipoExportacao = EnumTipoExportacao.ExportarTodosExcel;
            this.Close();
        }

        private void btnExportarSelecionado_Click(object sender, RoutedEventArgs e)
        {
            tipoExportacao = EnumTipoExportacao.ExportarSelecionado;
            this.Close();
        }
    }
}
