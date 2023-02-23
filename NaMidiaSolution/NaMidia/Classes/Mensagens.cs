using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaMidia.Classes
{
    public class Mensagens
    {
        public static string registroSalvoComSucesso = "Registro salvo com sucesso!";
        public static string registrosExportadoComSucesso = "Registros exportado eom sucesso!";
        public static string imagemSalvoComSucesso = "Imagem salva com sucesso!";
        public static string tituloMessageBox = "Na Midia Malharia";
        public static string registroExcluidoComSucesso = "Registro excluído com sucesso!";
        public static string selecioneUmRegistro = "Nenhum registro selecionado.";
        public static string desejaExcluirRegistro = "Deseja excluir o registro?";
        public static string erroSalvarRegistro = "Não foi possivel salvar o registro, se o problema persistir procure o administrador do sistema.";
        public static string erroExcluirRegistro = "O registro não pode ser excluído, verifique se o mesmo não esta sendo usado.";
        public static string erroCarregarRegistros = "Os registros não podem ser carregados, verifique a conexão com o Banco de Dados.";
        public static string desejaRealmenteCancelar = "Deseja realmente cancelar?";
        public static string desejaRealmenteExcluirVenda = "Deseja realmente excluir a venda?";
        public static string desejaRealmenteSair = "Deseja realmente sair?";
        public static string vendaNaoPodeSerAlterada = "Não é permitido a edição da venda, pois a venda foi relacionada com pagamentos ou costureiras.";
        public static string faltamInformacoes = "Faltam informações para completar o processo, por favor verifique as informações e tente novamente.";
        public static string faltamInformacoesVenda = " A venda não foi concluída, por favor verifique as informações e tente novamente.";
        public static string registroSalvoComSucessoImprimir = "Registro salvo com sucesso. Deseja imprimir o comprovante?";
        public static string pagamentoMaiorQueValorRestante = "O pagamento maior que o valor restante!";
        public static string graficoExporadoComSucesso = "Exportado com sucesso!";
        public static string vendaDesativadaComSucesso = "Venda desativada com sucesso!";
        public static string valorQuantidadeMaiorQueDisponivel = "O valor da quantidade é maior que o valor disponível!";
        public static string valorNaoPodeSer0 = "O valor tem que ser maior que 0!";
        public static string selecioneUmaData = "Selecione uma data.";
        public static string desmarcarDataEntrega = "Deseja desmarcar a data de entrega?";
        public static string edicaoVendaRelacionada = "Essa venda já possui relacionamentos. Se continuar com a edição, todos os dados, exceto os 'Itens da Venda' serão apagados. Deseja continuar?";
        public static string desativarVenda = "Deseja desativar a venda?";
        public static string erroExcluirNotificacao = "Não foi possivel excluir a notificação, se o problema persistir procure o administrador do sistema.";
        public static string excluirTodosRegistros = "Confirma a exclusão de todos os registros?";
        public static string DesejaImprimirComprovante = "Deseja imprimir comprovante?";
        public static string desejaPagarItensCostureiraJaPagos = "Há itens que ja foram pagos, se continuar a data de pagamento será alterada. Deseja continuar?";
        public static string desejaExcluirPagamentoCostureira = "Tem certeza que deseja excluir o pagamento para costureira?";
        public static string usuarioExiste = "Usuário ja existe! Por favor insira outro usuário.";
        public static string funcionarioJaRelacionado = "Não é possivel relacionar 2 usuários para um mesmo funcionário.";
        public static string usuarioOuSenhaInvalidos = "Usuário ou senha inválidos!.";
        public static string excluirLoginAdministrador = "Esse usuário é o administrador do sistema, não poderá ser excluido.";
        public static string caminhoInvalido = "Caminho inválido. Por favor selecione um caminho válido.";
        public static string backupSalvoComSucesso = "Backup salvo com sucesso!.";
    }
}
