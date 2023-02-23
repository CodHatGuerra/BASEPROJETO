using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class CategoriaDAL
    {
        public static void AlterarCategoria(CATEGORIA categoria)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var categoriaRecuperada = dc.CATEGORIAs.FirstOrDefault(a => a.cd_Categoria == categoria.cd_Categoria);

                if (categoriaRecuperada != null)
                    categoriaRecuperada.ds_Categoria = categoria.ds_Categoria;

                else
                    dc.CATEGORIAs.InsertOnSubmit(categoria);

                dc.SubmitChanges();
            }
        }

        public static void ExcluirCategoria(int categoriaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CATEGORIAs.DeleteOnSubmit(dc.CATEGORIAs.FirstOrDefault(a => a.cd_Categoria == categoriaId));
                dc.SubmitChanges();
            }
        }

        public static CATEGORIA RecuperarCategoria(int categoriaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.CATEGORIAs.Single(a => a.cd_Categoria == categoriaId);
        }

        public static List<CATEGORIA> RecuperarListaCategoria()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.CATEGORIAs.OrderBy(a => a.ds_Categoria).ToList();
        }
    }
}
