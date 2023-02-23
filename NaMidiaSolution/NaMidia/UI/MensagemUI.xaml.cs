using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NaMidia.UI
{
    public partial class MensagemUI : Window
    {
        DispatcherTimer tempo = null;
        int fecharJanelaEmSegundos = 6;

        public MensagemUI(string mensagem)
            : this(mensagem, "Imagens/concluido.png", 0)
        {
        }

        public MensagemUI(string mensagem, int tempoFechar)
            : this(mensagem, "Imagens/fecharNotificacao.png", tempoFechar)
        {
        }

        public MensagemUI(string mensagem, string caminhoImagem)
            : this(mensagem, caminhoImagem, 0)
        {
        }

        public MensagemUI(string mensagem, string caminhoImagem, int tempoFechar)
        {
            InitializeComponent();

            this.txtMensagem.Text = mensagem;

            this.image.Source = this.RecuperarImagem(caminhoImagem);

            if (tempoFechar != 0)
                this.fecharJanelaEmSegundos = tempoFechar;

                this.Left = 0;
                this.Top = (SystemParameters.WorkArea.Height - this.Height) - 2;
        }

        private void imgClose_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private ImageSource RecuperarImagem(string caminhoImagem)
        {
            string path = "pack://application:,,/" + caminhoImagem;

            ImageSourceConverter imgConv = new ImageSourceConverter();

            return (ImageSource)imgConv.ConvertFromString(path);
        }

        private void wndMensagem_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (this.tempo != null)
                {
                    this.tempo.Stop();
                }
            }
            catch
            {
            }
        }

        private void wndMensagem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.fecharJanelaEmSegundos != 0)
                {
                    this.tempo = new DispatcherTimer();
                    this.tempo.Interval = TimeSpan.FromSeconds(this.fecharJanelaEmSegundos);
                    this.tempo.Tick += new EventHandler(tempo_Tick);
                    this.tempo.Start();
                } 
            }
            catch
            {
            }
        }

        private void tempo_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Opacity = 0.3;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void wndMensagem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
