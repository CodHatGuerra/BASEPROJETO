using System;
namespace NaMidia.Classes
{
    public class ItensPagamentoCostureira
    {
        string nm_Costureira;
        public string Nm_Costureira
        {
            get { return nm_Costureira; }
            set { nm_Costureira = value; }
        }

        string nm_Cliente;
        public string Nm_Cliente
        {
            get { return nm_Cliente; }
            set { nm_Cliente = value; }
        }

        string ds_Produto;
        public string Ds_Produto
        {
            get { return ds_Produto; }
            set { ds_Produto = value; }
        }

        int cd_Venda;
        public int Cd_Venda
        {
            get { return cd_Venda; }
            set { cd_Venda = value; }
        }

        int ds_Quantidade;
        public int Ds_Quantidade
        {
            get { return ds_Quantidade; }
            set { ds_Quantidade = value; }
        }

        decimal ds_ValorUnitario;
        public decimal Ds_ValorUnitario
        {
            get { return ds_ValorUnitario; }
            set { ds_ValorUnitario = value; }
        }

        decimal ds_ValorTotal;
        public decimal Ds_ValorTotal
        {
            get { return ds_ValorTotal; }
            set { ds_ValorTotal = value; }
        }

        string ds_Malha;
        public string Ds_Malha
        {
            get { return ds_Malha; }
            set { ds_Malha = value; }
        }

        string ds_Tamanho;
        public string Ds_Tamanho
        {
            get { return ds_Tamanho; }
            set { ds_Tamanho = value; }
        }

        string ds_Gola;
        public string Ds_Gola
        {
            get { return ds_Gola; }
            set { ds_Gola = value; }
        }

        string ds_Observação;
        public string Ds_Observação
        {
            get { return ds_Observação; }
            set { ds_Observação = value; }
        }

        DateTime ds_DataPedido;
        public DateTime Ds_DataPedido
        {
            get { return ds_DataPedido; }
            set { ds_DataPedido = value; }
        }
    }
}
