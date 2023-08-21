using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Service.Interface;
using IronPdf;
using Ninject;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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

        public bool AlterarDebito(int codigoDebito, string anoDocumento, string numDocumento, string numProcesso, string valorMulta, string dataVencimento)
        {
            var dadosDebito = _debitoDapper.ObterDadosDebito(codigoDebito);

            if (dadosDebito == null)
                return false;

            dadosDebito.AnoDocumento = anoDocumento;
            dadosDebito.NumDocumento = numDocumento;
            dadosDebito.NumProcesso = numProcesso;
            dadosDebito.ValorDebito = valorMulta;
            dadosDebito.DataVencimento = dataVencimento;

            return _debitoDapper.AlterarDebito(codigoDebito, dadosDebito);
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

        public decimal CalculaNossoNumero(string uf, string receita, string tipoNossoNumero)
        {
            return _debitoDapper.CalculaNossoNumero(uf, receita, tipoNossoNumero);
        }

        public string GerarNotificacaoDebito(int codigoDebito, string nossoNumero, string valorMulta, string dataVencimento, string percentualSelic, string percentualMulta, string valorSelic, string valorMultaSelic)
        {
            try
            {
                string urlBoleto = string.Empty;

                var dadosDebito = _debitoDapper.ObterDadosDebito(codigoDebito);

                string instrucoes = $"Cota Única[br][br]Não receber após o vencimento.[br][br]Código do Débito: ' {codigoDebito.ToString()} '[br]Processo nº: {dadosDebito.NumProcesso} [br][br]Dúvidas: Central de Atendimento: 0800 642 9782";

                serviceRegistroBoleto.requisicaoBoletoRegistradoAvulso parametrosRequisicao = new serviceRegistroBoleto.requisicaoBoletoRegistradoAvulso();
                serviceRegistroBoleto.GuiaWebServiceClient guiaWS = new serviceRegistroBoleto.GuiaWebServiceClient();

                var valorDesconto = float.Parse(valorMulta) * (0.2);

                parametrosRequisicao.idSistema = "BAIXABB";
                parametrosRequisicao.numeroConvenio = "3547147";
                parametrosRequisicao.numeroCarteira = "17";
                parametrosRequisicao.numeroVariacaoCarteira = "477";
                parametrosRequisicao.codigoModalidadeTitulo = "1";
                parametrosRequisicao.dataEmissaoTitulo = "07.12.2022";
                parametrosRequisicao.dataVencimentoTitulo = "20.08.2023";
                parametrosRequisicao.valorOriginalTitulo = valorMulta;
                parametrosRequisicao.codigoTipoDesconto = "1";
                parametrosRequisicao.codigoTipoJuroMora = "0";
                parametrosRequisicao.codigoTipoMulta = "0";
                parametrosRequisicao.codigoAceiteTitulo = "N";
                parametrosRequisicao.codigoTipoTitulo = "4";
                parametrosRequisicao.codigoTipoContaCaucao = "0";
                parametrosRequisicao.textoNumeroTituloBeneficiario = "0";
                parametrosRequisicao.textoNumeroTituloCliente = "39809104038287511";
                parametrosRequisicao.codigoTipoInscricaoPagador = "2";
                parametrosRequisicao.numeroInscricaoPagador = "11111111000191";
                parametrosRequisicao.nomePagador = "clinkids servicos medicos ltda";
                parametrosRequisicao.textoEnderecoPagador = dadosDebito.Endereco;
                parametrosRequisicao.numeroCepPagador = dadosDebito.Cep;
                parametrosRequisicao.nomeMunicipioPagador = dadosDebito.Cidade;
                parametrosRequisicao.nomeBairroPagador = dadosDebito.Bairro;
                parametrosRequisicao.siglaUfPagador = dadosDebito.UF;
                parametrosRequisicao.codigoChaveUsuario = "1";
                parametrosRequisicao.codigoTipoCanalSolicitacao = "5";
                parametrosRequisicao.valorDescontoTitulo = valorDesconto.ToString();
                parametrosRequisicao.dataDescontoTitulo = "25.07.2023";

                var retorno = guiaWS.boletoAvulsoRegistradoBB(parametrosRequisicao);

                if (retorno != null && retorno.guiaArrecad != null)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://unigru-pre.anvisa.gov.br");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync($"/unigru/guia/{retorno.guiaArrecad.ano.ToString()}/{retorno.guiaArrecad.numero.ToString()}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var renderer = new ChromePdfRenderer();

                        var strContent = response.Content.ReadAsStringAsync().Result;

                        var pdf = renderer.RenderHtmlAsPdf(strContent);

                        pdf.SaveAs($"C:\\Anvisa\\CodivaServiceWS\\CodivaServiceWS\\CodivaServiceWS\\Boleto\\Boleto_{retorno.guiaArrecad.numero.ToString()}.pdf");
                    }
                }

                if(retorno != null && retorno.guiaArrecad != null)
                    urlBoleto = $"https://unigru-pre.anvisa.gov.br/unigru/guia/{retorno.guiaArrecad.ano.ToString()}/{retorno.guiaArrecad.numero.ToString()}";
                
                return urlBoleto;
            }
            catch (Exception ex)
            {
                return "";
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
