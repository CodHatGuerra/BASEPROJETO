using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class FormaPagamentoDAL
    {
        public static List<FORMAPAGAMENTO> RecuperarListaFormaPagamento()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.FORMAPAGAMENTOs.ToList();
        }
    }
}
