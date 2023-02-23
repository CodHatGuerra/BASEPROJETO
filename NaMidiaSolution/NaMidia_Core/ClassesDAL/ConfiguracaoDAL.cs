using System.Collections.Generic;
using System.Linq;
using NaMidiaCore.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class ConfiguracaoDAL
    {
        public static List<ViewLogPagamentoVenda> RecuperarLogsPagamentoVenda()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.ViewLogPagamentoVendas.ToList();
        }

        public static List<ViewLogVenda> RecuperarLogsVenda()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.ViewLogVendas.ToList();
        }
    }
}
