using NaMidiaCore.Linq;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace NaMidiaCore.ClassesDAL
{
    public class PessoaDAL
    {
        public static void AlterarPessoa(PESSOA pessoa, List<CONTATO> listaContato)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var pessoaRecuperada = dc.PESSOAs.FirstOrDefault(a => a.cd_Pessoa == pessoa.cd_Pessoa);

                if (pessoaRecuperada != null)
                {
                    pessoaRecuperada.nm_Pessoa = pessoa.nm_Pessoa;
                    pessoaRecuperada.nm_Fantasia = pessoa.nm_Fantasia;
                    pessoaRecuperada.ds_RazaoSocial = pessoa.ds_RazaoSocial;
                    pessoaRecuperada.ds_Endereco = pessoa.ds_Endereco;
                    pessoaRecuperada.ds_Numero = pessoa.ds_Numero;
                    pessoaRecuperada.ds_Bairro = pessoa.ds_Bairro;
                    pessoaRecuperada.cd_TipoPessoa = 2;
                    pessoaRecuperada.cd_Cidade = pessoa.cd_Cidade;
                    pessoaRecuperada.ds_Cnpj = pessoa.ds_Cnpj;
                    pessoaRecuperada.ds_Cpf = pessoa.ds_Cpf;
                    pessoaRecuperada.ds_Rg = pessoa.ds_Rg;

                    ExcluirContato(pessoa.cd_Pessoa);

                    listaContato.ForEach(novoContato =>
                    {
                        dc.CONTATOs.InsertOnSubmit(new CONTATO()
                        {
                            cd_Pessoa = pessoa.cd_Pessoa,
                            cd_TipoContato = novoContato.cd_TipoContato,
                            ds_Contato = novoContato.ds_Contato
                        });
                    });
                }

                else
                {
                    PESSOA pessoaNovo = new PESSOA();
                    pessoaNovo.nm_Pessoa = pessoa.nm_Pessoa;
                    pessoaNovo.nm_Fantasia = pessoa.nm_Fantasia;
                    pessoaNovo.ds_RazaoSocial = pessoa.ds_RazaoSocial;
                    pessoaNovo.ds_Endereco = pessoa.ds_Endereco;
                    pessoaNovo.ds_Numero = pessoa.ds_Numero;
                    pessoaNovo.ds_Bairro = pessoa.ds_Bairro;
                    pessoaNovo.cd_TipoPessoa = 2;
                    pessoaNovo.cd_Cidade = pessoa.cd_Cidade;
                    pessoaNovo.ds_Cnpj = pessoa.ds_Cnpj;
                    pessoaNovo.ds_Cpf = pessoa.ds_Cpf;
                    pessoaNovo.ds_Rg = pessoa.ds_Rg;
                    dc.PESSOAs.InsertOnSubmit(pessoaNovo);

                    listaContato.ForEach(novoContato =>
                    {
                        CONTATO contato = new CONTATO();
                        contato.PESSOA = pessoaNovo;
                        contato.cd_TipoContato = novoContato.cd_TipoContato;
                        contato.ds_Contato = novoContato.ds_Contato;
                        dc.CONTATOs.InsertOnSubmit(contato);
                    });
                }

                dc.SubmitChanges();
            }
        }

        public static void ExcluirPessoa(int pessoaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                //Apagando todos os contatos relacionados com essa pessoa
                dc.CONTATOs.DeleteAllOnSubmit((from a in dc.CONTATOs
                                               where a.cd_Pessoa == pessoaId
                                               select a).ToList());

                dc.PESSOAs.DeleteOnSubmit(dc.PESSOAs.FirstOrDefault(a => a.cd_Pessoa == pessoaId));
                dc.SubmitChanges();
            }
        }

        private static void ExcluirContato(int pessoaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CONTATOs.DeleteAllOnSubmit((from a in dc.CONTATOs
                                               where a.cd_Pessoa == pessoaId
                                               select a));
                dc.SubmitChanges();
            }
        }

        private static void InserirContato(CONTATO contato)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.CONTATOs.InsertOnSubmit(contato);
                dc.SubmitChanges();
            }
        }

        public static List<PESSOA> RecuperarListaPessoa()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<PESSOA>(a => a.CIDADE);

            return dc.PESSOAs.OrderBy(a => a.nm_Pessoa).ToList();
        }

        public static List<TIPOCONTATO> RecuperarListaContato()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.TIPOCONTATOs.OrderBy(a => a.nm_TipoContato).ToList();
        }

        public static List<TIPOPESSOA> RecuperarListaTipoPessoa()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.TIPOPESSOAs.OrderBy(a => a.ds_TipoPessoa).ToList();
        }

        public static PESSOA RecuperarPessoa(int pessoaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<PESSOA>(a => a.CIDADE);
            dc.LoadOptions = option;

            return dc.PESSOAs.Single(a => a.cd_Pessoa == pessoaId);
        }

        public static List<CONTATO> RecuperarListaContatoPessoa(int pessoaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.CONTATOs where a.cd_Pessoa == pessoaId select a).ToList();
        }

        public static int RecuperarQuantidadeContato(int pessoaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.CONTATOs
                    where a.cd_Pessoa == pessoaId && a.cd_TipoContato != 4
                    select a).Count();
        }

        public static CONTATO RecuperarContato(int pessoaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.CONTATOs.FirstOrDefault(a => a.cd_Pessoa == pessoaId && a.cd_TipoContato != 4);
        }
    }
}
