using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class FuncionarioDAL
    {
        public static void AlterarFuncionario(FUNCIONARIO funcionario)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var funcionarioRecuperado = dc.FUNCIONARIOs.FirstOrDefault(a => a.cd_Funcionario == funcionario.cd_Funcionario);

                if (funcionarioRecuperado != null)
                {
                    funcionarioRecuperado.cd_Cargo = funcionario.cd_Cargo;
                    funcionarioRecuperado.cd_Cidade = funcionario.cd_Cidade;
                    funcionarioRecuperado.ds_Bairro = funcionario.ds_Bairro;
                    funcionarioRecuperado.ds_Cpf = funcionario.ds_Cpf;
                    funcionarioRecuperado.ds_Email = funcionario.ds_Email;
                    funcionarioRecuperado.ds_Endereco = funcionario.ds_Endereco;
                    funcionarioRecuperado.ds_Imagem = funcionario.ds_Imagem;
                    funcionarioRecuperado.ds_Numero = funcionario.ds_Numero;
                    funcionarioRecuperado.ds_Rg = funcionario.ds_Rg;
                    funcionarioRecuperado.ds_Salario = funcionario.ds_Salario;
                    funcionarioRecuperado.ds_Telefone = funcionario.ds_Telefone;
                    funcionarioRecuperado.dt_Entrada = funcionario.dt_Entrada;
                    funcionarioRecuperado.dt_Saida = funcionario.dt_Saida;
                    funcionarioRecuperado.nm_Funcionario = funcionario.nm_Funcionario;
                    funcionarioRecuperado.statusFuncionario = funcionario.statusFuncionario;
                }

                else
                    dc.FUNCIONARIOs.InsertOnSubmit(funcionario);

                dc.SubmitChanges();
            }
        }

        public static void ExcluiFuncionario(int funcionarioId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.FUNCIONARIOs.DeleteOnSubmit(dc.FUNCIONARIOs.FirstOrDefault(a => a.cd_Funcionario == funcionarioId));
                dc.SubmitChanges();
            }
        }

        public static List<CARGO> RecuperarListaCargo()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.CARGOs.OrderBy(a => a.ds_Cargo).ToList();
        }

        public static List<FUNCIONARIO> RecuperarListaFuncionario()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.FUNCIONARIOs.OrderBy(a => a.nm_Funcionario).ToList();
        }

        public static FUNCIONARIO RecuperarFuncionario(int funcionarioId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.FUNCIONARIOs.Single(a => a.cd_Funcionario == funcionarioId);
        }
    }
}
