using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NaMidia.Classes;
using NaMidiaCore.ClassesDAL;
using Telerik.Windows.Controls;

namespace NaMidia.UI
{

    public partial class PesquisaDataEntregaVenda : Window
    {
        #region [Propriedades]

        int vendaId;
        public bool concluiu = false;
        bool marcarEntrega;

        #endregion

        #region [Contrutor]

        public PesquisaDataEntregaVenda(int vendaId, bool marcarEntrega)
        {
            this.vendaId = vendaId;
            this.marcarEntrega = marcarEntrega;

            InitializeComponent();
            this.txtCdVenda.Text = vendaId.ToString();
            this.dtDataEntrega.SelectedDate = DateTime.Now;
        }

        #endregion

        #region [Eventos]

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AlterarDataEntregaVenda();
            }
            catch (Exception ex)
            {
                concluiu = false;
                throw new Exception(ex.Message);
            }
        }

        private void btnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            concluiu = false;
            this.Close();
        }

        #endregion

        #region [Metodos]

        public void AlterarDataEntregaVenda()
        {
            VendaDAL.AtualizarStatusEntregaVenda(vendaId, Convert.ToDateTime(dtDataEntrega.SelectedDate), marcarEntrega);
            new MensagemUI(Mensagens.registroSalvoComSucesso).Show();
            concluiu = true;
            this.Close();
        }
        #endregion
    }
}
