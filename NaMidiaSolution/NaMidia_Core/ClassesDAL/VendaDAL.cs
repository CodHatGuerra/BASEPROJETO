using NaMidiaCore.Classes;
using NaMidiaCore.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Transactions;

namespace NaMidiaCore.ClassesDAL
{
    public enum enumAlteracaoImagemVenda { Novo = 1, Edicao = 2 };

    public class VendaDAL
    {
        public static List<PAGAMENTOVENDA> RecuperarListaPagamentoVenda(int vendaId)
        {
            try
            {
                NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
                return dc.PAGAMENTOVENDAs.Where(a => a.cd_Venda == vendaId).ToList();
            }

            catch (Exception)
            {
                return null;
            }
        }

        public static List<ViewRelatorioPagamento> RecuperarListaPagamentoVenda()
        {
            try
            {
                NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
                return dc.ViewRelatorioPagamentos.ToList();
            }

            catch (Exception)
            {
                return null;
            }
        }

        public static Decimal? ResgatarValorFuncCalcularReajuste(int cdPagamentoVenda)
        {
            return new NaMidiaContextDataContext().funcCalcularReajuste(cdPagamentoVenda);
        }

        public static void AlterarVenda(VENDA venda, List<ITENSVENDA> listaItensVenda, List<PAGAMENTOVENDA> listaPagamentoVenda, IMAGEMVENDA imagemVenda, int funcionarioId)
        {
            using (TransactionScope transition = new TransactionScope())
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    var vendaRecuperada = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == venda.cd_Venda);

                    if (vendaRecuperada != null)
                    {
                        vendaRecuperada.cd_TipoPedido = venda.cd_TipoPedido;
                        vendaRecuperada.cd_Funcionario = venda.cd_Funcionario;
                        vendaRecuperada.ds_Observacoes = venda.ds_Observacoes;
                        vendaRecuperada.statusVenda = venda.statusVenda;
                        vendaRecuperada.statusPagamentoVenda = venda.statusPagamentoVenda;
                        vendaRecuperada.dt_DataPrevista = venda.dt_DataPrevista;
                        vendaRecuperada.cd_Funcionario = funcionarioId;
                        vendaRecuperada.cd_FormaPagamento = venda.cd_FormaPagamento;
                        vendaRecuperada.statusPedidoCostureira = false;
                        vendaRecuperada.statusEntregaVenda = false;
                        dc.SubmitChanges();

                        int cd_Venda = vendaRecuperada.cd_Venda;

                        //Salvar imagem
                        var imagemRecuperada = dc.IMAGEMVENDAs.FirstOrDefault(a => a.cd_Venda == imagemVenda.cd_Venda);

                        if (imagemRecuperada != null)
                        {
                            imagemRecuperada.ds_Imagem = new RecortarImagem().RecuperarImagemRecortada(imagemVenda.ds_Imagem.ToArray(), 142, 313);
                            dc.SubmitChanges();
                        }

                        dc.ITENSVENDAs.DeleteAllOnSubmit((from a in dc.ITENSVENDAs
                                                          where a.cd_Venda == vendaRecuperada.cd_Venda
                                                          select a).ToList());
                        dc.SubmitChanges();

                        listaItensVenda.ForEach(itens =>
                        {
                            dc.ITENSVENDAs.InsertOnSubmit(new ITENSVENDA()
                            {
                                cd_Venda = cd_Venda,
                                ds_Quantidade = itens.ds_Quantidade,
                                cd_Produto = itens.cd_Produto,
                                cd_Tamanho = itens.cd_Tamanho,
                                cd_Malha = itens.cd_Malha,
                                cd_Gola = itens.cd_Gola,
                                ds_ValorUnitario = itens.ds_ValorUnitario,
                                ds_SubTotal = itens.ds_SubTotal,
                                ds_Observacoes = itens.ds_Observacoes
                            });
                            dc.SubmitChanges();
                        });

                        dc.PAGAMENTOVENDAs.DeleteAllOnSubmit((from a in dc.PAGAMENTOVENDAs
                                                              where a.cd_Venda == vendaRecuperada.cd_Venda
                                                              select a).ToList());
                        dc.SubmitChanges();

                        listaPagamentoVenda.ForEach(pgm =>
                        {
                            pgm.cd_Venda = cd_Venda;
                            dc.PAGAMENTOVENDAs.InsertOnSubmit(pgm);
                            dc.SubmitChanges();

                            if (venda.cd_FormaPagamento == (int)EnumFormaPagamento.Cartão_de_Crédito)
                            {
                                dc.PAGAMENTOVENDASREGISTROs.InsertOnSubmit(new PAGAMENTOVENDASREGISTRO()
                                {
                                    cd_PagamentoVenda = RecuperarUltimoPagamentoVenda(dc),
                                    ds_ValorPagamento = pgm.ds_ValorRecebido,
                                    dt_Pagamento = DateTime.Now,
                                });
                                dc.SubmitChanges();
                            }
                        });
                    }

                    else
                    {
                        venda.cd_Funcionario = funcionarioId;
                        venda.dt_Data = DateTime.Now;
                        venda.statusPedidoCostureira = false;
                        venda.statusEntregaVenda = false;

                        dc.VENDAs.InsertOnSubmit(venda);
                        dc.SubmitChanges();

                        int cd_Venda = (from a in dc.VENDAs select a.cd_Venda).Max();

                        //Salvar imagem
                        imagemVenda.cd_Venda = cd_Venda;
                        imagemVenda.ds_Imagem = new RecortarImagem().RecuperarImagemRecortada(imagemVenda.ds_Imagem.ToArray(), 142, 313);
                        dc.IMAGEMVENDAs.InsertOnSubmit(imagemVenda);
                        dc.SubmitChanges();

                        listaItensVenda.ForEach(itens =>
                        {
                            dc.ITENSVENDAs.InsertOnSubmit(new ITENSVENDA()
                            {
                                cd_Venda = cd_Venda,
                                ds_Quantidade = itens.ds_Quantidade,
                                cd_Produto = itens.cd_Produto,
                                cd_Tamanho = itens.cd_Tamanho,
                                cd_Malha = itens.cd_Malha,
                                cd_Gola = itens.cd_Gola,
                                ds_ValorUnitario = itens.ds_ValorUnitario,
                                ds_SubTotal = itens.ds_SubTotal,
                                ds_Observacoes = itens.ds_Observacoes
                            });
                            dc.SubmitChanges();
                        });

                        listaPagamentoVenda.ForEach(pgm =>
                        {
                            pgm.cd_Venda = cd_Venda;
                            dc.PAGAMENTOVENDAs.InsertOnSubmit(pgm);
                            dc.SubmitChanges();

                            if (venda.cd_FormaPagamento == (int)EnumFormaPagamento.Cartão_de_Crédito)
                            {
                                dc.PAGAMENTOVENDASREGISTROs.InsertOnSubmit(new PAGAMENTOVENDASREGISTRO()
                                {
                                    cd_PagamentoVenda = RecuperarUltimoPagamentoVenda(dc),
                                    ds_ValorPagamento = pgm.ds_ValorRecebido,
                                    dt_Pagamento = DateTime.Now
                                });
                                dc.SubmitChanges();
                            }
                        });
                    }
                }

                transition.Complete();
            }
        }

        public static void AlterarVenda(VENDA venda, List<ITENSVENDA> listaItensVenda, IMAGEMVENDA imagemVenda, int funcionarioId)
        {
            using (TransactionScope transition = new TransactionScope())
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    var vendaRecuperada = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == venda.cd_Venda);

                    if (vendaRecuperada != null)
                    {
                        vendaRecuperada.cd_TipoPedido = venda.cd_TipoPedido;
                        vendaRecuperada.ds_Observacoes = venda.ds_Observacoes;
                        vendaRecuperada.statusVenda = venda.statusVenda;
                        vendaRecuperada.statusPagamentoVenda = venda.statusPagamentoVenda;
                        vendaRecuperada.dt_DataPrevista = venda.dt_DataPrevista;
                        vendaRecuperada.cd_Funcionario = funcionarioId;
                        dc.SubmitChanges();

                        int cd_Venda = vendaRecuperada.cd_Venda;

                        //Salvar imagem
                        var imagemRecuperada = dc.IMAGEMVENDAs.FirstOrDefault(a => a.cd_Venda == imagemVenda.cd_Venda);

                        if (imagemRecuperada != null)
                        {
                            imagemRecuperada.ds_Imagem = new RecortarImagem().RecuperarImagemRecortada(imagemVenda.ds_Imagem.ToArray(), 142, 313);
                            dc.SubmitChanges();
                        }

                        dc.ITENSVENDAs.DeleteAllOnSubmit((from a in dc.ITENSVENDAs
                                                          where a.cd_Venda == vendaRecuperada.cd_Venda
                                                          select a).ToList());
                        dc.SubmitChanges();

                        listaItensVenda.ForEach(itens =>
                        {
                            dc.ITENSVENDAs.InsertOnSubmit(new ITENSVENDA()
                            {
                                cd_Venda = cd_Venda,
                                ds_Quantidade = itens.ds_Quantidade,
                                cd_Produto = itens.cd_Produto,
                                cd_Tamanho = itens.cd_Tamanho,
                                cd_Malha = itens.cd_Malha,
                                cd_Gola = itens.cd_Gola,
                                ds_ValorUnitario = itens.ds_ValorUnitario,
                                ds_SubTotal = itens.ds_SubTotal,
                                ds_Observacoes = itens.ds_Observacoes
                            });
                            dc.SubmitChanges();
                        });

                        dc.PAGAMENTOVENDAs.DeleteAllOnSubmit((from a in dc.PAGAMENTOVENDAs
                                                              where a.cd_Venda == vendaRecuperada.cd_Venda
                                                              select a).ToList());
                        dc.SubmitChanges();
                    }

                    else
                    {
                        venda.cd_Funcionario = funcionarioId;
                        venda.dt_Data = DateTime.Now;
                        dc.VENDAs.InsertOnSubmit(venda);
                        dc.SubmitChanges();

                        int cd_Venda = (from a in dc.VENDAs select a.cd_Venda).Max();

                        //Salvar imagem
                        imagemVenda.cd_Venda = cd_Venda;
                        imagemVenda.ds_Imagem = new RecortarImagem().RecuperarImagemRecortada(imagemVenda.ds_Imagem.ToArray(), 142, 313);
                        dc.IMAGEMVENDAs.InsertOnSubmit(imagemVenda);
                        dc.SubmitChanges();

                        listaItensVenda.ForEach(itens =>
                        {
                            dc.ITENSVENDAs.InsertOnSubmit(new ITENSVENDA()
                            {
                                cd_Venda = cd_Venda,
                                ds_Quantidade = itens.ds_Quantidade,
                                cd_Produto = itens.cd_Produto,
                                cd_Tamanho = itens.cd_Tamanho,
                                cd_Malha = itens.cd_Malha,
                                cd_Gola = itens.cd_Gola,
                                ds_ValorUnitario = itens.ds_ValorUnitario,
                                ds_SubTotal = itens.ds_SubTotal,
                                ds_Observacoes = itens.ds_Observacoes
                            });
                            dc.SubmitChanges();
                        });
                    }
                }

                transition.Complete();
            }
        }

        public static void ExcluirVenda(VENDA venda)
        {
            using (TransactionScope transition = new TransactionScope())
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    dc.PAGAMENTOVENDAs.Where(a => a.cd_Venda == venda.cd_Venda).ToList().ForEach(pagamentoVenda =>
                    {
                        dc.PAGAMENTOVENDASREGISTROs.DeleteAllOnSubmit(dc.PAGAMENTOVENDASREGISTROs.Where(a => a.cd_PagamentoVenda == pagamentoVenda.cd_PagamentoVenda).ToList());
                        dc.PAGAMENTOVENDAs.DeleteAllOnSubmit(dc.PAGAMENTOVENDAs.Where(a => a.cd_PagamentoVenda == pagamentoVenda.cd_PagamentoVenda).ToList());
                    });

                    dc.ITENSVENDAs.Where(a => a.cd_Venda == venda.cd_Venda).ToList().ForEach(itensVenda =>
                    {
                        dc.COSTUREIRAPEDIDOs.DeleteAllOnSubmit(dc.COSTUREIRAPEDIDOs.Where(a => a.cd_ItensVenda == itensVenda.cd_ItensVenda).ToList());
                        dc.ITENSVENDAs.DeleteAllOnSubmit(dc.ITENSVENDAs.Where(a => a.cd_ItensVenda == itensVenda.cd_ItensVenda).ToList());
                    });

                    dc.IMAGEMVENDAs.DeleteOnSubmit(dc.IMAGEMVENDAs.FirstOrDefault(a => a.cd_Venda == venda.cd_Venda));
                    dc.VENDAs.DeleteOnSubmit(dc.VENDAs.FirstOrDefault(a => a.cd_Venda == venda.cd_Venda));
                    dc.SubmitChanges();
                }
                transition.Complete();
            }
        }

        public static VENDA RecuperarVenda(int vendaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.VENDAs.Single(a => a.cd_Venda == vendaId);
        }

        public static IMAGEMVENDA RecuperarImagemVenda(int vendaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.IMAGEMVENDAs.Single(a => a.cd_Venda == vendaId);
        }

        public static PAGAMENTOVENDA RecuperarPagamentoVenda(int pagamentoVendaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                return dc.PAGAMENTOVENDAs.Single(a => a.cd_PagamentoVenda == pagamentoVendaId);
        }

        public static List<VENDA> RecuperarListaVendas()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<VENDA>(a => a.PESSOA);
            option.LoadWith<VENDA>(a => a.ITENSVENDAs);
            dc.LoadOptions = option;

            return dc.VENDAs.Where(a => a.statusVenda == true && a.cd_TipoPedido == 2).ToList(); // Retorna todas as vendas que são ativas e tipo Venda
        }
        public static List<VENDA> RecuperarListaVendasOrcamento()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<VENDA>(a => a.PESSOA);
            option.LoadWith<VENDA>(a => a.ITENSVENDAs);
            dc.LoadOptions = option;

            return dc.VENDAs.Where(a => a.statusVenda == true).ToList(); // Retorna todas as vendas que são ativas e tipo Venda
        }


        public static List<VENDA> RecuperarListaVendas(EnumStatusVenda.StatusVenda statusVendaPagamento) // Retorna vendas quitadas ou não quitadas
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return statusVendaPagamento == EnumStatusVenda.StatusVenda.Quitada ? dc.VENDAs.Where(a => a.statusPagamentoVenda == true && a.statusVenda == true).ToList() : dc.VENDAs.Where(a => a.statusPagamentoVenda != true && a.statusVenda == true).ToList();
        }

        public static List<ViewRelatorioVenda> RecuperarRelatorioVenda()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return dc.ViewRelatorioVendas.ToList();
        }

        public static List<ITENSVENDA> RecuperarListaItensVenda(int vendaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<ITENSVENDA>(a => a.GOLA);
            option.LoadWith<ITENSVENDA>(a => a.MALHA);
            option.LoadWith<ITENSVENDA>(a => a.PRODUTO);
            option.LoadWith<ITENSVENDA>(a => a.TAMANHO);
            dc.LoadOptions = option;

            return (from a in dc.ITENSVENDAs
                    where a.cd_Venda == vendaId
                    select a).ToList();
        }

        public static bool VerificarEdicaoVenda(int vendaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();

            List<PAGAMENTOVENDA> listaPagamentoVenda = (from a in dc.PAGAMENTOVENDAs
                                                        where a.cd_Venda == vendaId && a.ds_ValorRecebido != 0
                                                        select a).ToList();

            foreach (var pagamentoVenda in listaPagamentoVenda)
            {
                if (pagamentoVenda.ds_ValorRecebido != null || pagamentoVenda.ds_ValorRecebido != 0)
                    return false;
            }

            List<ITENSVENDA> listaItensVenda = (from a in dc.ITENSVENDAs
                                                where a.cd_Venda == vendaId
                                                select a).ToList();

            foreach (var itensVenda in listaItensVenda)
            {
                if (dc.COSTUREIRAPEDIDOs.Where(a => a.cd_ItensVenda == itensVenda.cd_ItensVenda).Count() > 0)
                    return false;
            }

            return true;
        }

        public static List<PAGAMENTOVENDA> RecuperarParcelasVenda(int vendaID)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<PAGAMENTOVENDA>(a => a.PAGAMENTOVENDASREGISTROs);
            dc.LoadOptions = option;

            return (from a in dc.PAGAMENTOVENDAs
                    where a.cd_Venda == vendaID
                    select a).ToList();
        }

        public static List<PAGAMENTOVENDASREGISTRO> RecuperPagamentosParcela(int pagamentoVendaId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.PAGAMENTOVENDASREGISTROs
                    where a.cd_PagamentoVenda == pagamentoVendaId
                    select a).ToList();
        }

        public static void AtualizarStatusEntregaVenda(int vendaId, DateTime dataEntrega, bool statusEntrega)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                VENDA vendaRecuperada = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == vendaId);
                vendaRecuperada.statusEntregaVenda = statusEntrega;
                if (statusEntrega)
                    vendaRecuperada.dt_DataEntrega = dataEntrega;

                else
                    vendaRecuperada.dt_DataEntrega = null;

                dc.SubmitChanges();
            }
        }

        public static void AlterarPagamentoVenda(PAGAMENTOVENDA pagamentoVenda, PAGAMENTOVENDASREGISTRO pagamentoVendaRegistro, int funcionarioId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                PAGAMENTOVENDA pagamento = new PAGAMENTOVENDA();
                pagamento = dc.PAGAMENTOVENDAs.Single(a => a.cd_PagamentoVenda == pagamentoVenda.cd_PagamentoVenda);

                pagamento.ds_ValorRecebido = pagamentoVenda.ds_ValorRecebido;
                pagamento.ds_ValorRestante = pagamentoVenda.ds_ValorRestante;
                pagamento.dt_Pagamento_Efetuado = DateTime.Now;
                pagamento.ds_ValorReajuste = pagamentoVenda.ds_ValorReajuste;
                pagamento.ds_ValorReajustado = pagamentoVenda.ds_ValorReajustado;
                pagamento.cd_Reajuste = pagamentoVenda.cd_Reajuste;
                pagamento.statusPagamentoParcela = pagamento.ds_ValorRestante == 0 ? true : false;

                dc.SubmitChanges();

                AlterarPagamentoVendaRegistro(pagamentoVendaRegistro, funcionarioId);
            }
        }

        public static void AlterarPagamentoVenda(PAGAMENTOVENDA pagamentoVenda)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                PAGAMENTOVENDA pagamento = new PAGAMENTOVENDA();
                pagamento = dc.PAGAMENTOVENDAs.Single(a => a.cd_PagamentoVenda == pagamentoVenda.cd_PagamentoVenda);

                pagamento.ds_ValorRecebido = pagamentoVenda.ds_ValorRecebido;
                pagamento.ds_ValorRestante = pagamentoVenda.ds_ValorRestante;
                pagamento.dt_Pagamento_Efetuado = DateTime.Now;
                pagamento.ds_ValorReajuste = pagamentoVenda.ds_ValorReajuste;
                pagamento.ds_ValorReajustado = pagamentoVenda.ds_ValorReajustado;
                pagamento.cd_Reajuste = pagamentoVenda.cd_Reajuste;
                pagamento.statusPagamentoParcela = pagamento.ds_ValorRestante == 0 ? true : false;

                dc.SubmitChanges();
            }
        }

        public static void AlterarPagamentoVendaRegistro(PAGAMENTOVENDASREGISTRO pagamentoVendaRegistro, int funcionarioId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                var pagamentoVendaRegistroRecuperada = dc.PAGAMENTOVENDASREGISTROs.FirstOrDefault(a => a.cd_PagamentoVendaRegistros == pagamentoVendaRegistro.cd_PagamentoVendaRegistros);

                if (pagamentoVendaRegistroRecuperada != null)
                {
                    pagamentoVendaRegistroRecuperada.ds_ValorPagamento = pagamentoVendaRegistro.ds_ValorPagamento;
                    pagamentoVendaRegistroRecuperada.dt_Pagamento = pagamentoVendaRegistro.dt_Pagamento;
                    pagamentoVendaRegistroRecuperada.cd_Funcionario = funcionarioId;
                }

                else
                {
                    pagamentoVendaRegistro.cd_Funcionario = funcionarioId;
                    dc.PAGAMENTOVENDASREGISTROs.InsertOnSubmit(pagamentoVendaRegistro);
                }

                dc.SubmitChanges();
            }
        }

        public static void ExcluirPagamentoVendaRegistro(PAGAMENTOVENDASREGISTRO pagamentoVendaRegistro)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                dc.PAGAMENTOVENDASREGISTROs.DeleteOnSubmit(dc.PAGAMENTOVENDASREGISTROs.FirstOrDefault(a => a.cd_PagamentoVendaRegistros == pagamentoVendaRegistro.cd_PagamentoVendaRegistros));
                dc.SubmitChanges();
            }
        }

        public static void DesativarVenda(int vendaId)
        {
            using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
            {
                VENDA venda = dc.VENDAs.FirstOrDefault(a => a.cd_Venda == vendaId);
                venda.statusVenda = false;
                dc.SubmitChanges();
            }
        }

        public static void ExcluirCascataVenda(int vendaId)
        {
            using (TransactionScope transition = new TransactionScope())
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    // Apagando Pagamentos_Registros
                    dc.PAGAMENTOVENDAs.Where(a => a.cd_Venda == vendaId).ToList().ForEach(pagamentoVenda =>
                    {
                        dc.PAGAMENTOVENDASREGISTROs.DeleteAllOnSubmit(dc.PAGAMENTOVENDASREGISTROs.Where(a => a.cd_PagamentoVenda == pagamentoVenda.cd_PagamentoVenda));
                        dc.PAGAMENTOVENDAs.DeleteOnSubmit(pagamentoVenda);
                    });

                    // Apagando registros Costureira
                    dc.ITENSVENDAs.Where(a => a.cd_Venda == vendaId).ToList().ForEach(itensVenda =>
                    {
                        dc.COSTUREIRAPEDIDOs.DeleteAllOnSubmit(dc.COSTUREIRAPEDIDOs.Where(a => a.cd_ItensVenda == itensVenda.cd_ItensVenda));
                    });

                    dc.SubmitChanges();
                }

                transition.Complete();
            }
        }

        public static int RecuperarUltimaVenda()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.VENDAs select a.cd_Venda).Max();
        }

        public static int RecuperarUltimoPagamentoVenda(NaMidiaContextDataContext dc)
        {
            return (from a in dc.PAGAMENTOVENDAs select a.cd_PagamentoVenda).Max();
        }

        public static PAGAMENTOVENDASREGISTRO RecuperarPagamentoVendaRegistro(int pagamentoVendaRegistroId)
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            DataLoadOptions option = new DataLoadOptions();
            option.LoadWith<PAGAMENTOVENDASREGISTRO>(a => a.PAGAMENTOVENDA);
            option.LoadWith<PAGAMENTOVENDA>(a => a.VENDA);
            option.LoadWith<VENDA>(a => a.PESSOA);
            dc.LoadOptions = option;

            return dc.PAGAMENTOVENDASREGISTROs.FirstOrDefault(a => a.cd_PagamentoVendaRegistros == pagamentoVendaRegistroId);
        }

        public static int RecuperarUltimoPagamentovendaRegistro()
        {
            NaMidiaContextDataContext dc = new NaMidiaContextDataContext();
            return (from a in dc.PAGAMENTOVENDASREGISTROs select a.cd_PagamentoVendaRegistros).Max();
        }

        public static void OtimizarImagens()
        {
            using (TransactionScope transition = new TransactionScope())
            {
                using (NaMidiaContextDataContext dc = new NaMidiaContextDataContext())
                {
                    foreach (IMAGEMVENDA imagemVenda in dc.IMAGEMVENDAs.ToList())
                        imagemVenda.ds_Imagem = new RecortarImagem().RecuperarImagemRecortada(imagemVenda.ds_Imagem.ToArray(), 142, 313);

                    dc.SubmitChanges();
                }

                transition.Complete();
            }
        }
    }
}