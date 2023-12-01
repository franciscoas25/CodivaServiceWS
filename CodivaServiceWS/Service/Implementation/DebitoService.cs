using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Service.Interface;
using IronPdf;
using Ninject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using static CodivaServiceWS.Enum.Enum;
using BrazilHolidays;
using BrazilHolidays.Net;
using System.Web;
using BoletoNetCore;
using Grpc.Core.Utils;
using System.Text;
using CodivaServiceWS.serviceRegistroBoleto;

namespace CodivaServiceWS.Service.Implementation
{
    public class DebitoService : IDebitoService
    {
        [Inject]
        public IBanco _banco;

        [Inject]
        public IDebitoDapper _debitoDapper { get; set; }

        [Inject]
        public IBaseDapper _baseDapper { get; set; }

        public bool IncluirDebito(string cpf_cnpj, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta)
        {
            var dataVencimentoDebito = CalculaDataVencimentoTitulo().ToString();

            var codigoPessoaDevedora = _baseDapper.ObterCodigoPessoaDevedoraPorCpfCnpj(cpf_cnpj);

            var codTipoDebito = tipoDebito.ToLower() == TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description.ToLower() ? 1 : 14;

            return _debitoDapper.IncluirDebito(codigoPessoaDevedora, receita, 0, unidadeArrecadadora, anoDocumento, numDocumento, numProcesso, codTipoDebito, dataMulta, valorMulta, dataVencimentoDebito);
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

        public (bool sucesso, string urlBoleto, string nossoNumero, double codigoBoletoRegistrado, int numeroGuia) GerarNotificacaoDebito(int codigoDebito, string valorMulta, string dataVencimento, string percentualSelic, string percentualMulta, string valorSelic, string valorMultaSelic)
        {
            string urlBoleto = string.Empty;
            string nossoNumero = string.Empty;
            double codigoBoletoRegistrado = 0;
            int numeroGuia = 0;

            try
            {
                List<BoletoBancario> lstBoletos = new List<BoletoBancario>();

                var dadosDebito = _debitoDapper.ObterDadosDebito(codigoDebito);

                if (dadosDebito == null)
                    return (false, urlBoleto, nossoNumero, codigoBoletoRegistrado, numeroGuia);

                string instrucoes = $"Cota Única[br][br]Não receber após o vencimento.[br][br]Código do Débito: {codigoDebito} [br]Processo nº: {dadosDebito.NumProcesso} [br][br]Dúvidas: Central de Atendimento: 0800 642 9782";

                serviceRegistroBoleto.requisicaoBoletoRegistradoAvulso parametrosRequisicao = new serviceRegistroBoleto.requisicaoBoletoRegistradoAvulso();
                serviceRegistroBoleto.GuiaWebServiceClient guiaWS = new serviceRegistroBoleto.GuiaWebServiceClient();

                //serviceRegistroBoletoProducao.requisicaoBoletoRegistradoAvulso parametrosRequisicao = new serviceRegistroBoletoProducao.requisicaoBoletoRegistradoAvulso();
                //serviceRegistroBoletoProducao.GuiaWebServiceClient guiaWS = new serviceRegistroBoletoProducao.GuiaWebServiceClient();

                var valorDesconto = float.Parse(valorMulta) * (0.2);

                parametrosRequisicao.idSistema = "BAIXABB";
                parametrosRequisicao.numeroConvenio = "3547147";
                parametrosRequisicao.numeroCarteira = "17";
                parametrosRequisicao.numeroVariacaoCarteira = "477";
                parametrosRequisicao.codigoModalidadeTitulo = "1";
                parametrosRequisicao.dataEmissaoTitulo = "23.06.2023";
                parametrosRequisicao.dataVencimentoTitulo = CalculaDataVencimentoTitulo().ToShortDateString().Replace("/", ".");
                parametrosRequisicao.valorOriginalTitulo = valorMulta;
                parametrosRequisicao.codigoTipoDesconto = "1";
                parametrosRequisicao.codigoTipoJuroMora = "0";
                parametrosRequisicao.codigoTipoMulta = "0";
                parametrosRequisicao.codigoAceiteTitulo = "N";
                parametrosRequisicao.codigoTipoTitulo = "4";
                parametrosRequisicao.codigoTipoContaCaucao = "0";
                parametrosRequisicao.textoNumeroTituloBeneficiario = "0";
                parametrosRequisicao.textoNumeroTituloCliente = parametrosRequisicao.numeroConvenio + codigoDebito.ToString().PadLeft(8, '0') + DateTime.Now.ToString("yy");
                parametrosRequisicao.textoMensagemBloquetoOcorrencia = instrucoes;
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
                parametrosRequisicao.dataDescontoTitulo = CalculaDataLimitePagamentoDesconto().ToShortDateString().Replace("/", ".");

                var retorno = guiaWS.boletoAvulsoRegistradoBB(parametrosRequisicao);

                if (retorno == null || retorno.guiaArrecad == null)
                    return (false, urlBoleto, nossoNumero, codigoBoletoRegistrado, numeroGuia);

                string ano = retorno.guiaArrecad.numero.ToString().Length > 6 ? DateTime.Now.ToString("yyyy").Substring(0, 3) : DateTime.Now.ToString("yyyy");

                nossoNumero = parametrosRequisicao.numeroConvenio + retorno.guiaArrecad.numero.ToString() + ano;

                var contaBancaria = new ContaBancaria()
                {
                    Agencia = "3477",
                    DigitoAgencia = "0",
                    Conta = "00333001",
                    DigitoConta = "X",
                    OperacaoConta = "019",
                    CarteiraPadrao = "17",
                    VariacaoCarteiraPadrao = "019"
                };

                var cedente = new Beneficiario()
                {
                    Codigo = parametrosRequisicao.numeroConvenio,
                    CPFCNPJ = "03112386000111",
                    Nome = "AGÊNCIA NACIONAL DE VIGILÂNCIA SANITÁRIA - ANVISA",
                    ContaBancaria = contaBancaria
                };

                var sacado = new Pagador()
                {
                    CPFCNPJ = parametrosRequisicao.numeroInscricaoPagador,
                    Nome = parametrosRequisicao.nomePagador,
                    Endereco = new Endereco()
                    {
                        LogradouroEndereco = dadosDebito.Endereco,
                        Bairro = dadosDebito.Bairro,
                        Cidade = dadosDebito.Cidade,
                        UF = dadosDebito.UF,
                        CEP = dadosDebito.Cep
                    }
                };

                _banco = Banco.Instancia(Bancos.BancoDoBrasil);
                _banco.Beneficiario = cedente;
                _banco.FormataBeneficiario();

                var boleto = new Boleto(_banco)
                {
                    DataVencimento = CalculaDataVencimentoTitulo(),
                    ValorTitulo = Convert.ToDecimal(valorMulta),
                    NossoNumero = nossoNumero,
                    NumeroDocumento = "0000000085",
                    EspecieDocumento = TipoEspecieDocumento.DS,
                    Pagador = sacado,
                    Carteira = "17",
                    DataDesconto = CalculaDataLimitePagamentoDesconto(),
                    ValorDesconto = Convert.ToDecimal(valorDesconto),
                    ImprimirMensagemInstrucao = true,
                    MensagemInstrucoesCaixaFormatado = "Desconto de 20% se pago até 20 dias a contar da data de recebimento desta notificação, nos termos do art. 21 da Lei n. 6.437/77",
                    MensagemInstrucoesCaixa = "Desconto de 20% se pago até 20 dias a contar da data de recebimento desta notificação, nos termos do art. 21 da Lei n. 6.437/77",
                };

                var boleto_bancario = new BoletoBancario()
                {
                    Boleto = boleto,
                    MostrarCodigoCarteira = false,
                    MostrarComprovanteEntrega = false,
                    OcultarReciboPagador = true
                };

                boleto_bancario.Boleto.ValidarDados();

                lstBoletos.Add(boleto_bancario);

                var bytes = boleto_bancario.MontaBytesListaBoletosPDF(lstBoletos, codigoDebito.ToString());

                string diretorioBoleto = System.Web.HttpContext.Current.Server.MapPath("./Boleto");

                if (!Directory.Exists(diretorioBoleto))
                    Directory.CreateDirectory(diretorioBoleto);

                string pathBoleto = Path.Combine(diretorioBoleto, codigoDebito.ToString() + "_Boleto.pdf");

                using (var fs = new FileStream(pathBoleto, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                return (true, urlBoleto, nossoNumero, retorno.codigoBoletoRegistrado, retorno.guiaArrecad.numero);
            }
            catch (Exception ex)
            {
                return (false, urlBoleto, nossoNumero, codigoBoletoRegistrado, numeroGuia);
            }
        }

        private DateTime CalculaDataLimitePagamentoDesconto()
        {
            DateTime diaUtilSeguinteDataEmissao = DateTime.Now.DayOfWeek == DayOfWeek.Friday ?
                                                  DateTime.Now.AddDays(3) :
                                                  DateTime.Now.DayOfWeek == DayOfWeek.Saturday ?
                                                  DateTime.Now.AddDays(2) :
                                                  DateTime.Now.AddDays(1);

            var dataLimitePagamentoComDesconto = diaUtilSeguinteDataEmissao.AddDays(20);

            DateTime ultimoDiaDoMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var ultimoDiaUtilDoMes = ultimoDiaDoMes.DayOfWeek == DayOfWeek.Saturday ?
                                     ultimoDiaDoMes.AddDays(-1) :
                                     ultimoDiaDoMes.DayOfWeek == DayOfWeek.Sunday ?
                                     ultimoDiaDoMes.AddDays(-2) :
                                     ultimoDiaDoMes;

            if (dataLimitePagamentoComDesconto > ultimoDiaUtilDoMes)
                dataLimitePagamentoComDesconto = ultimoDiaUtilDoMes;

            return dataLimitePagamentoComDesconto;
        }

        private DateTime CalculaDataVencimentoTitulo()
        {
            DateTime ultimoDiaDoMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var ultimoDiaUtilDoMes = ultimoDiaDoMes;

            for (DateTime data = ultimoDiaDoMes; data.Day > 0; data = data.AddDays(-1))
            {
                if (IsDiaUtil(data))
                {
                    ultimoDiaUtilDoMes = data;
                    break;
                }
            }

            return ultimoDiaUtilDoMes;
        }

        private Boolean IsDiaUtil(DateTime dt)
        {
            return !dt.IsHoliday() && dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday;
        }

        public bool IncluirParcelaDebito(int codigoDebito, string nossoNumero, string dataVencimento, string valorMulta)
        {
            return _debitoDapper.IncluirParcelaDebito(codigoDebito, nossoNumero, dataVencimento, valorMulta);
        }

        public bool AtualizaNossoNumero(double codigoBoletoRegistrado, string nossoNumero)
        {
            return _debitoDapper.AtualizaNossoNumero(codigoBoletoRegistrado, nossoNumero);
        }

        public bool AtualizarSituacaoDebito(int codigoDebito, int codigoSituacao)
        {
            return _debitoDapper.AtualizarSituacaoDebito(codigoDebito, codigoSituacao);
        }

        public bool IncluirHistoricoSituacaoDebito(int codDebito, int coStatusDebito, string coUsuario)
        {
            return _debitoDapper.IncluirHistoricoSituacaoDebito(codDebito, coStatusDebito, coUsuario);
        }

        public void GravarMensagem(string mensagem)
        {
            var path = HttpContext.Current.Server.MapPath("/log/");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString() + ".txt";

            StreamWriter sw = new StreamWriter(path);

            sw.WriteLine(DateTime.Now.ToString() + " " + mensagem);

            sw.Close();
        }
    }
}
