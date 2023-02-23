using System.Windows;

namespace NaMidia
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new LoginUI().Show();
            base.OnStartup(e);
        }
    }
}
