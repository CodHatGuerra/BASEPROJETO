using NaMidia.Classes;
using NaMidiaCore.Classes;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace NaMidia.ClassesDAL
{
    public class PerfilDAL
    {
        private static Dictionary<Tuple<int, int>, int> dicPerfilModulo { get; set; } = new Dictionary<Tuple<int, int>, int>();

        public static void AtualizarModuloAcao(NaMidiaContextDataContext dc, TreeViewModulo treeViewModulo, int codigoPerfilModulo)
        {
            if (treeViewModulo.leitura)
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Leitura).FirstOrDefault();
                if (moduloAcao == null)
                    dc.MODULO_ACAOs.InsertOnSubmit(new MODULO_ACAO { cd_PerfilModulo = codigoPerfilModulo, cd_Acao = (int)AcaoEnum.Leitura });
            }
            else
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Leitura).FirstOrDefault();
                if (moduloAcao != null)
                    dc.MODULO_ACAOs.DeleteOnSubmit(moduloAcao);
            }

            if (treeViewModulo.inserir)
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Inserir).FirstOrDefault();
                if (moduloAcao == null)
                    dc.MODULO_ACAOs.InsertOnSubmit(new MODULO_ACAO { cd_PerfilModulo = codigoPerfilModulo, cd_Acao = (int)AcaoEnum.Inserir });
            }
            else
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Inserir).FirstOrDefault();
                if (moduloAcao != null)
                    dc.MODULO_ACAOs.DeleteOnSubmit(moduloAcao);
            }

            if (treeViewModulo.editar)
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Editar).FirstOrDefault();
                if (moduloAcao == null)
                    dc.MODULO_ACAOs.InsertOnSubmit(new MODULO_ACAO { cd_PerfilModulo = codigoPerfilModulo, cd_Acao = (int)AcaoEnum.Editar });
            }
            else
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Editar).FirstOrDefault();
                if (moduloAcao != null)
                    dc.MODULO_ACAOs.DeleteOnSubmit(moduloAcao);
            }
            if (treeViewModulo.remover)
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Remover).FirstOrDefault();
                if (moduloAcao == null)
                    dc.MODULO_ACAOs.InsertOnSubmit(new MODULO_ACAO { cd_PerfilModulo = codigoPerfilModulo, cd_Acao = (int)AcaoEnum.Remover });
            }
            else
            {
                var moduloAcao = dc.MODULO_ACAOs.Where(a => a.cd_PerfilModulo == codigoPerfilModulo && a.cd_Acao == (int)AcaoEnum.Remover).FirstOrDefault();
                if (moduloAcao != null)
                    dc.MODULO_ACAOs.DeleteOnSubmit(moduloAcao);
            }

            dc.SubmitChanges();
        }

        public static void AlterarPerfil(PERFIL perfil, List<TreeViewModulo> lstModulos)
        {
            using (TransactionScope transition = new TransactionScope())
            {
                int codigoPerfilModulo = 0;

                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    var perfilRecuperado = dc.PERFILs.FirstOrDefault(a => a.cd_Perfil == perfil.cd_Perfil);

                    if (perfilRecuperado != null)
                    {
                        perfilRecuperado.ds_Perfil = perfil.ds_Perfil;
                        dc.SubmitChanges();
                    }

                    else
                    {
                        int? cd_Perfil = dc.PERFILs.Select(a => a.cd_Perfil).Max();
                        perfil.cd_Perfil = cd_Perfil == null ? 1 : (cd_Perfil.Value + 1);
                        dc.PERFILs.InsertOnSubmit(perfil);
                    }

                    Func<TreeViewModulo, TreeViewModulo> funcModulo = null;
                    funcModulo = (modulo =>
                    {
                        TreeViewModulo treeViewModulo = new TreeViewModulo();

                        foreach (var item in modulo.items.ToList())
                        {
                            codigoPerfilModulo = AtualizarPerfilModulo(dc, perfil.cd_Perfil, item.cd_Modulo);
                            AtualizarModuloAcao(dc, item, codigoPerfilModulo);
                            funcModulo(item);
                        }

                        codigoPerfilModulo = AtualizarPerfilModulo(dc, perfil.cd_Perfil, modulo.cd_Modulo);
                        AtualizarModuloAcao(dc, modulo, codigoPerfilModulo);
                        return treeViewModulo;
                    });

                    foreach (var item in lstModulos)
                        funcModulo(item);

                    dc.SubmitChanges();
                }

                transition.Complete();
            }
        }
        
        public static int AtualizarPerfilModulo(NaMidiaContextDataContext dc, int codigoPerfil, int codigoModulo)
        {
            if (dicPerfilModulo.ContainsKey(new Tuple<int, int>(codigoPerfil, codigoModulo)))
                return dicPerfilModulo[new Tuple<int, int>(codigoPerfil, codigoModulo)];

            var PerfilModulo = dc.PERFIL_MODULOs.Where(a => a.cd_Perfil == codigoPerfil && a.cd_Modulo == codigoModulo).FirstOrDefault();

            if (PerfilModulo == null)
            {
                PerfilModulo = new PERFIL_MODULO { cd_Modulo = codigoModulo, cd_Perfil = codigoPerfil };
                dc.PERFIL_MODULOs.InsertOnSubmit(PerfilModulo);
                dc.SubmitChanges();
            }

            return PerfilModulo.cd_PerfilModulo;
        }

        public static void ExcluirPerfil(int perfilId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.PERFILs.DeleteOnSubmit(dc.PERFILs.FirstOrDefault(a => a.cd_Perfil == perfilId));
                dc.SubmitChanges();
            }
        }

        public static List<PERFIL> RecuperarListaPerfil()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.PERFILs.OrderBy(a => a.ds_Perfil).ToList();
        }

        public static PERFIL RecuperarPerfil(int perfilId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.PERFILs.Single(a => a.cd_Perfil == perfilId);
        }

        public static List<TreeViewModulo> RecuperarListaTreeViewModulo(int? codigoPerfil)
        {
            List<TreeViewModulo> lista = new List<TreeViewModulo>();
            var listaAux = new List<ProcRecuperarPerfilModuloAcaoResult>();

            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                listaAux = dc.ProcRecuperarPerfilModuloAcao(codigoPerfil).ToList();

            Func<ProcRecuperarPerfilModuloAcaoResult, TreeViewModulo> funcModulo = null;

            funcModulo = (modulo =>
            {
                TreeViewModulo treeViewModulo = new TreeViewModulo();

                foreach (var item in listaAux.Where(a => a.cd_ModuloPai == modulo.cd_Modulo).OrderBy(a => a.ds_OrdemExibicao).ToList())
                {
                    treeViewModulo.cd_Modulo = modulo.cd_Modulo;
                    treeViewModulo.cd_ModuloPai = modulo.cd_ModuloPai;
                    treeViewModulo.ds_Modulo = modulo.ds_Modulo;
                    treeViewModulo.ds_OrdemExibicao = Convert.ToInt32(modulo.ds_OrdemExibicao);
                    treeViewModulo.editar = Convert.ToBoolean(modulo.editar);
                    treeViewModulo.inserir = Convert.ToBoolean(modulo.inserir);
                    treeViewModulo.leitura = Convert.ToBoolean(modulo.leitura);
                    treeViewModulo.remover = Convert.ToBoolean(modulo.remover);
                    treeViewModulo.items.Add(funcModulo(item));
                }

                treeViewModulo.cd_Modulo = modulo.cd_Modulo;
                treeViewModulo.cd_ModuloPai = modulo.cd_ModuloPai;
                treeViewModulo.ds_Modulo = modulo.ds_Modulo;
                treeViewModulo.ds_OrdemExibicao = Convert.ToInt32(modulo.ds_OrdemExibicao);
                treeViewModulo.editar = Convert.ToBoolean(modulo.editar);
                treeViewModulo.inserir = Convert.ToBoolean(modulo.inserir);
                treeViewModulo.leitura = Convert.ToBoolean(modulo.leitura);
                treeViewModulo.remover = Convert.ToBoolean(modulo.remover);

                return treeViewModulo;
            });

            foreach (var item in listaAux.Where(a => a.cd_ModuloPai == null).OrderBy(a => a.ds_OrdemExibicao).ToList())
                lista.Add(funcModulo(item));

            return lista;
        }
    }
}
