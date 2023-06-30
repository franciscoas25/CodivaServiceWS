using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Service.Interface;
using CodivaServiceWS.serviceRegistroBoleto;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS.Service.Implementation
{
    public class DebitoService : IDebitoService
    {
        [Inject]
        public IDebitoDapper _debitoDapper { get; set; }

        [Inject]
        public IBaseDapper _baseDapper { get; set; }

        public bool IncluirDebito(string cpf_cnpj, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta)
        {
            var codigoPessoaDevedora = _baseDapper.ObterCodigoPessoaDevedoraPorCpfCnpj(cpf_cnpj);

            var codTipoDebito = tipoDebito.ToLower() == TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description.ToLower() ? 1 : 14;

            return _debitoDapper.IncluirDebito(codigoPessoaDevedora, receita, 0, unidadeArrecadadora, anoDocumento, numDocumento, numProcesso, codTipoDebito, valorMulta);
        }

        public bool ValidaDebito(string cpf_cnpj, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta)
        {
            return !string.IsNullOrEmpty(cpf_cnpj) &&
                   !string.IsNullOrEmpty(tipoDebito) &&
                   !string.IsNullOrEmpty(numDocumento) &&
                   !string.IsNullOrEmpty(anoDocumento) &&
                   !string.IsNullOrEmpty(numProcesso) &&
                   !string.IsNullOrEmpty(gerencia) &&
                   !string.IsNullOrEmpty(nomePessoa) &&
                   !string.IsNullOrEmpty(receita) &&
                   unidadeArrecadadora > 0 &&
                   !string.IsNullOrEmpty(valorMulta) &&
                   DateTime.TryParse(dataMulta, out DateTime result);
        }

        public bool VerificaSeDebitoEstaCadastrado(string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora)
        {
            return _debitoDapper.VerificaSeDebitoEstaCadastrado(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora);
        }

        public bool IncluirHistoricoSituacaoDebito(int codigoDebito, string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora)
        {
            var codTipoDebito = tipoDebito.ToLower() == TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description.ToLower() ? 1 : 14;

            return _debitoDapper.IncluirHistoricoSituacaoDebito(codigoDebito, 1, "PAULO.ALBUQUERQUE");
        }

        public int ObterCodigoDebito(string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora)
        {
            var codTipoDebito = tipoDebito.ToLower() == TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description.ToLower() ? 1 : 14;

            return _baseDapper.ObterCodigoDebito(codTipoDebito, numDocumento, anoDocumento, unidadeArrecadadora);
        }

        public bool CalculaNossoNumero(string uf, string receita, string tipoNossoNumero)
        {
            return _debitoDapper.CalculaNossoNumero(uf, receita, tipoNossoNumero);
        }

        public void GerarNotificacaoDebito(int codigoDebito, string nossoNumero, string valorMulta, string dataVencimento, string percentualSelic, string percentualMulta, string valorSelic, string valorMultaSelic)
        {
            try
            {
                var dadosDebito = _debitoDapper.ObterDadosDebito(codigoDebito);

                string instrucoes = $"Cota Única[br][br]Não receber após o vencimento.[br][br]Código do Débito: ' {codigoDebito.ToString()} '[br]Processo nº: {dadosDebito.NumProcesso} [br][br]Dúvidas: Central de Atendimento: 0800 642 9782";

                serviceRegistroBoleto.requisicaoBoletoRegistradoAvulso parametrosRequisicao = new serviceRegistroBoleto.requisicaoBoletoRegistradoAvulso();
                serviceRegistroBoleto.GuiaWebServiceClient guiaWS = new serviceRegistroBoleto.GuiaWebServiceClient();

                parametrosRequisicao.idSistema = "CODIVA";
                //parametrosRequisicao.numeroConvenio = getNumConvenio(dadosDebito.SiglaDebito, "COTAUNICA");
                parametrosRequisicao.numeroConvenio = "3547147";
                parametrosRequisicao.numeroCarteira = "17";
                //parametrosRequisicao.numeroVariacaoCarteira = getVariacaoCarteira(dadosDebito.SiglaDebito, "COTAUNICA");
                parametrosRequisicao.numeroVariacaoCarteira = "477";
                parametrosRequisicao.codigoModalidadeTitulo = "1";
                //parametrosRequisicao.dataEmissaoTitulo = DateTime.Now.ToString("dd/MM/yyyy");
                parametrosRequisicao.dataEmissaoTitulo = "07.12.2022";
                parametrosRequisicao.dataVencimentoTitulo = "12.07.2023";
                parametrosRequisicao.valorOriginalTitulo = valorMulta;
                parametrosRequisicao.codigoTipoDesconto = "1";
                parametrosRequisicao.codigoTipoJuroMora = "0";
                parametrosRequisicao.codigoTipoMulta = "0";
                parametrosRequisicao.codigoAceiteTitulo = "N";
                parametrosRequisicao.codigoTipoTitulo = "4";
                parametrosRequisicao.codigoTipoContaCaucao = "0";
                //parametrosRequisicao.indicadorPermissaoRecebimentoParcial = "S";
                parametrosRequisicao.textoNumeroTituloBeneficiario = "0";
                parametrosRequisicao.textoNumeroTituloCliente = "39809104038287511";
                //parametrosRequisicao.textoMensagemBloquetoOcorrencia = instrucoes; //instrucoes.Substring(0, 400);
                //parametrosRequisicao.codigoTipoInscricaoPagador = (int)TipoPessoa.PessoaFisica == 11 ? "1" : "2";
                parametrosRequisicao.codigoTipoInscricaoPagador = "2";
                //parametrosRequisicao.numeroInscricaoPagador = dadosDebito.CpfCnpj;
                parametrosRequisicao.numeroInscricaoPagador = "11111111000191";
                //parametrosRequisicao.nomePagador = dadosDebito.NomePessoa;
                parametrosRequisicao.nomePagador = "clinkids servicos medicos ltda";
                parametrosRequisicao.textoEnderecoPagador = dadosDebito.Endereco;
                parametrosRequisicao.numeroCepPagador = dadosDebito.Cep;
                parametrosRequisicao.nomeMunicipioPagador = dadosDebito.Cidade;
                parametrosRequisicao.nomeBairroPagador = dadosDebito.Bairro;
                parametrosRequisicao.siglaUfPagador = dadosDebito.UF;
                parametrosRequisicao.codigoChaveUsuario = "1";
                //parametrosRequisicao.codigoTipoCanalSolicitacao = "S";

                var retorno = guiaWS.boletoAvulsoRegistradoBB(parametrosRequisicao);
            }
            catch (Exception ex)
            {

            }
        }

        private String getNumConvenio(String sTpDebito, String sFormaPagamento)
        {
            if ((sTpDebito.Equals("AI") || sTpDebito.Equals("OD") || sTpDebito.Equals("AC")) && (sFormaPagamento.Equals("COTAUNICA")))
                return "2941053";
            else if ((sTpDebito.Equals("AI") || sTpDebito.Equals("OD") || sTpDebito.Equals("AC")) && (sFormaPagamento.Equals("PARCELADO")))
                return "2941054";
            else if (sTpDebito.Equals("AF") || sTpDebito.Equals("TX") || sTpDebito.Equals("RE") || sTpDebito.Equals("FU") || sTpDebito.Equals("CH") || sTpDebito.Equals("TC") || sTpDebito.Equals("TS"))
                return "2941051";
            else
                return "";
        }

        private String getVariacaoCarteira(String sTpDebito, String sFormaPagamento)
        {
            if ((sTpDebito.Equals("AI") || sTpDebito.Equals("OD") || sTpDebito.Equals("AC")) && (sFormaPagamento.Equals("COTAUNICA")))
                return "140";
            else if ((sTpDebito.Equals("AI") || sTpDebito.Equals("OD") || sTpDebito.Equals("AC")) && (sFormaPagamento.Equals("PARCELADO")))
                return "167";
            else if (sTpDebito.Equals("AF") || sTpDebito.Equals("TX") || sTpDebito.Equals("RE") || sTpDebito.Equals("FU") || sTpDebito.Equals("CH") || sTpDebito.Equals("TC") || sTpDebito.Equals("TS"))
                return "108";
            else
                return "";
        }
    }
}
