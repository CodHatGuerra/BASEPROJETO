using NaMidiaCore.ClassesDAL;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NaMidia.Classes
{
    public class CostureiraConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is ITENSVENDA)
                {
                    if (CostureiraPedidoDAL.RecuperarQuantidadeProdutoDisponivel(value as ITENSVENDA) == 0)
                        return "Green";

                    else
                        return "Red";
                }

                else if (value is COSTUREIRAPEDIDO)
                {
                    if ((value as COSTUREIRAPEDIDO).statusPagamento == true)
                        return "Green";

                    else
                        return "Red";
                }

                else if (value is ViewRelatorioCostureira)
                {
                    if ((value as ViewRelatorioCostureira).statusPagamento == true)
                        return "Green";

                    else
                        return "Red";
                }

                else if (value is VENDA)
                {
                    if ((value as VENDA).dt_DataPrevista <= DateTime.Today)
                        return "Red";

                    else
                        return "Yellow";
                }

                else if (value is PAGAMENTOVENDA)
                {
                    if ((value as PAGAMENTOVENDA).dt_Pagamento_Prevista <= DateTime.Today)
                        return "Red";

                    else
                        return "Yellow";
                }

                else
                    return string.Empty;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Mensagens.tituloMessageBox, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception();
        }
    }
}
