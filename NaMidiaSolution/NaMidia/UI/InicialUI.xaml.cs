using NaMidia.Classes;
using System.Windows;
using System.Windows.Controls;

namespace NaMidia.UI
{
    public partial class InicialUI : UserControl
    {
        public InicialUI()
        {
            InitializeComponent();
            lblUsuario.Content = Gerenciador.UsuarioAtivo.FUNCIONARIO.nm_Funcionario + ",";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
