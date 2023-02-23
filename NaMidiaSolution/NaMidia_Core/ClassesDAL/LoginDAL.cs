using NaMidia.Classes;
using NaMidiaCore.Classes;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace NaMidiaCore.ClassesDAL
{
    public class LoginDAL
    {
        public static LOGIN VerificarLogin(string usuario, string senha)
        {

            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<LOGIN>(a => a.FUNCIONARIO);
            dc.LoadOptions = option;

            List<LOGIN> listaLogin = (from a in dc.LOGINs
                                      where a.ds_Usuario == usuario && a.ds_Senha == Criptografia.Criptografar(senha, true)
                                      select a).ToList();

            if (listaLogin.Count > 0)
                return listaLogin[0];

            else
                return null;
        }

        public static void AlterarLogin(LOGIN login, List<PerfilUsuarioCombo> lista)
        {
            using (TransactionScope transition = new TransactionScope())
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    var loginRecuperado = dc.LOGINs.FirstOrDefault(a => a.cd_Login == login.cd_Login);
                    int codigoUsuario = 0;

                    if (loginRecuperado != null)
                    {
                        loginRecuperado.cd_Funcionario = login.cd_Funcionario;
                        loginRecuperado.ds_Usuario = login.ds_Usuario;
                        loginRecuperado.ds_Senha = Criptografia.Criptografar(login.ds_Senha, true);
                        loginRecuperado.cd_Perfil = login.cd_Perfil;
                        codigoUsuario = loginRecuperado.cd_Login;
                        dc.SubmitChanges();
                    }

                    else
                    {
                        login.ds_Senha = Criptografia.Criptografar(login.ds_Senha, true);
                        dc.LOGINs.InsertOnSubmit(login);
                        dc.SubmitChanges();
                        codigoUsuario = login.cd_Login;
                    }

                    foreach (PerfilUsuarioCombo item in lista)
                        AlterarPerfilUsuario(dc, codigoUsuario, item.cd_Perfil, item.isChecked);
                }

                transition.Complete();
            }
        }

        public static void ExcluirLogin(int loginId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.PERFIL_USUARIOs.DeleteAllOnSubmit(dc.PERFIL_USUARIOs.Where(a => a.cd_Usuario == loginId));
                dc.LOGINs.DeleteOnSubmit(dc.LOGINs.FirstOrDefault(a => a.cd_Login == loginId));
                dc.SubmitChanges();
            }
        }

        private static void AlterarPerfilUsuario(NaMidiaContextDataContext dc, int loginId, int perfilId, bool adicionar)
        {
            var perfilUsuario = dc.PERFIL_USUARIOs.Where(a => a.cd_Perfil == perfilId && a.cd_Usuario == loginId).FirstOrDefault();
            if (perfilUsuario == null && adicionar)
            {
                dc.PERFIL_USUARIOs.InsertOnSubmit(new PERFIL_USUARIO { cd_Perfil = perfilId, cd_Usuario = loginId });
                dc.SubmitChanges();
            }
            else if (perfilUsuario != null && !adicionar)
            {
                dc.PERFIL_USUARIOs.DeleteOnSubmit(perfilUsuario);
                dc.SubmitChanges();
            }
        }

        public static List<LOGIN> RecuperarListaLogin()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.LOGINs.ToList();
        }

        public static List<FUNCIONARIO> RecuperarListaFuncionarios()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.FUNCIONARIOs.ToList();
        }

        public static List<PerfilUsuarioCombo> RecuperarListaPerfilUsuario(int codigoUsuario)
        {
            using (SqlConnection connection = new SqlConnection(new NaMidiaContextDataContext().Connection.ConnectionString))
            {
                List<PerfilUsuarioCombo> lista = new List<PerfilUsuarioCombo>();

                string sql = "SELECT P.cd_Perfil,P.ds_Perfil,CASE WHEN PU.cd_Perfil IS NULL THEN 0 ELSE 1 END AS isChecked FROM PERFIL P LEFT JOIN PERFIL_USUARIO PU ON P.cd_Perfil = PU.cd_Perfil AND PU.cd_Usuario = @CodigoUsuario";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CodigoUsuario", codigoUsuario);

                connection.Open();

                SqlDataReader row = command.ExecuteReader();
                while (row.Read())
                {
                    lista.Add(new PerfilUsuarioCombo
                    {
                        cd_Perfil = Convert.ToInt32(row["cd_Perfil"]),
                        ds_Perfil = row["ds_Perfil"].ToString(),
                        isChecked = Convert.ToBoolean(row["isChecked"])
                    });
                }
                row.Close();

                return lista;
            }
        }

        public static LOGIN RecuperarLogin(int loginId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.LOGINs.Single(a => a.cd_Login == loginId);
        }

        public static bool VerificarFuncionarioJaRelacionado(LOGIN login)
        {
            try
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                    return (dc.LOGINs.Where(a => a.cd_Funcionario == login.cd_Funcionario && a.cd_Login != login.cd_Login).ToList().Count > 0) ? true : false;
            }

            catch (Exception)
            {
                return true;
            }
        }

        public static bool VerificarUsuarioExiste(LOGIN login)
        {
            try
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                    return (dc.LOGINs.Where(a => a.ds_Usuario == login.ds_Usuario && a.cd_Login != login.cd_Login).ToList().Count() > 0) ? true : false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
