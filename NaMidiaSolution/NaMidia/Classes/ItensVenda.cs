using System.ComponentModel;
namespace NaMidia.Classes
{
    public class ItensVenda:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int cd_ItenVenda;
        public int Cd_ItenVenda
        {
            get { return cd_ItenVenda; }
            set
            {
                cd_ItenVenda = value;
                OnPropertyChanged("cd_ItenVenda");
            }
        }

        private int cd_Pedido;
        public int Cd_Pedido
        {
            get { return cd_Pedido; }
            set
            {
                cd_Pedido = value;
                OnPropertyChanged("cd_Pedido");
            }
        }

        private int quantidade;
        public int Quantidade
        {
            get { return quantidade; }
            set
            {
                quantidade = value;
                OnPropertyChanged("quantidade");
            }
        }

        private int cd_Tamanho;
        public int Cd_Tamanho
        {
            get { return cd_Tamanho; }
            set
            {
                cd_Tamanho = value;
                OnPropertyChanged("cd_Tamanho");
            }
        }

        private string ds_Tamanho;
        public string Ds_Tamanho
        {
            get { return ds_Tamanho; }
            set
            {
                ds_Tamanho = value;
                OnPropertyChanged("ds_Tamanho");
            }
        }

        private int cd_Malha;
        public int Cd_Malha
        {
            get { return cd_Malha; }
            set
            {
                cd_Malha = value;
                OnPropertyChanged("cd_Malha");
            }
        }

        private string ds_Malha;
        public string Ds_Malha
        {
            get { return ds_Malha; }
            set
            {
                ds_Malha = value;
                OnPropertyChanged("ds_Malha");
            }
        }

        private int cd_Gola;
        public int Cd_Gola
        {
            get { return cd_Gola; }
            set
            {
                cd_Gola = value;
                OnPropertyChanged("cd_Gola");
            }
        }

        private string ds_Gola;
        public string Ds_Gola
        {
            get { return ds_Gola; }
            set
            {
                ds_Gola = value;
                OnPropertyChanged("ds_Gola");
            }
        }

        private int cd_Produto;
        public int Cd_Produto
        {
            get { return cd_Produto; }
            set
            {
                cd_Produto = value;
                OnPropertyChanged("cd_Produto");
            }
        }

        private string ds_Produto;
        public string Ds_Produto
        {
            get { return ds_Produto; }
            set
            {
                ds_Produto = value;
                OnPropertyChanged("ds_Produto");
            }
        }

        private double valorUnitario;
        public double ValorUnitario
        {
            get { return valorUnitario; }
            set
            {
                valorUnitario = value;
                OnPropertyChanged("valorUnitario");
            }
        }

        private double valorTotal;
        public double ValorTotal
        {
            get { return valorTotal; }
            set
            {
                valorTotal = value;
                OnPropertyChanged("valorTotal");
            }
        }

        private string ds_Observacoes;
        public string Ds_Observacoes
        {
            get { return ds_Observacoes; }
            set
            {
                ds_Observacoes = value;
                OnPropertyChanged("ds_Observacoes");
            }
        }

        private byte[] imagem;
        public byte[] Imagem
        {
            get { return imagem; }
            set
            {
                imagem = value;
                OnPropertyChanged("imagem");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        
    }
}
