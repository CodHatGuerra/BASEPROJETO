using System.Collections.Generic;

namespace NaMidia.Classes
{
    public class TreeViewModulo
    {
        public TreeViewModulo()
        {
            this.items = new List<TreeViewModulo>();
        }
        public int cd_Modulo { get; set; }
        public int? cd_ModuloPai { get; set; }
        public int ds_OrdemExibicao { get; set; }
        public string ds_Modulo { get; set; }
        public bool leitura { get; set; }
        public bool inserir { get; set; }
        public bool editar { get; set; }
        public bool remover { get; set; }

        public List<TreeViewModulo> items;
    }
}
