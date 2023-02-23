using System;
using System.Collections.Generic;
using System.Linq;                                                                                                                  
using System.Text;
using System.Threading.Tasks;

namespace NaMidia.Classes
{
    class ItensPagamentoVenda
    {
        decimal? valorParcela;
        public decimal? ValorParcela
        {
            get { return valorParcela; }
            set { valorParcela = value; }
        }

        decimal? valorRecebido;
        public decimal? ValorRecebido
        {
            get { return valorRecebido; }
            set { valorRecebido = value; }
        }

        decimal? valorRestante;
        public decimal? ValorRestante
        {
            get { return valorRestante; }
            set { valorRestante = value; }
        }

        decimal? valorReajuste;
        public decimal? ValorReajuste
        {
            get { return valorReajuste; }
            set { valorReajuste = value; }
        }

        string ds_Reajuste;
        public string Ds_Reajuste
        {
            get { return ds_Reajuste; }
            set { ds_Reajuste = value; }
        }

        DateTime? dataPagamento;
        public DateTime? DataPagamento
        {
            get { return dataPagamento; }
            set { dataPagamento = value; }
        }

        DateTime? dataPrevista;
        public DateTime? DataPrevista
        {
            get { return dataPrevista; }
            set { dataPrevista = value; }
        }
    }
}
