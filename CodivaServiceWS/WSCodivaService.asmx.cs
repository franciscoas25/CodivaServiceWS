using CodivaServiceWS.Dto;
using CodivaServiceWS.Service.Interface;
using Ninject;
using Ninject.Web;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.UI.Design;
using System.Xml;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS
{
    [WebService(Namespace = "http://tempuri.org/")]    
    public class WSCodivaService : WebServiceBase
    {
        [Inject]
        public IPessoaAutuadaService _pessoaAutuadaService { get; set; }
        [Inject]
        public IDebitoService _debitoService { get; set; }

        [WebMethod]
        public IncluirDebitoResponseDto IncluirDebito(Debitos debitos)
        {
            IncluirDebitoResponseDto incluirDebitoResponseDto = new IncluirDebitoResponseDto();

            try
            {
                GravarMensagem("Iniciando cadastro de débito...");

                Random randNum = new Random();

                //cpf_cnpj = "03583856872";
                //sistemaOrigem = "SEI";
                debitos.tipoDebito = "";
                debitos.numDocumento = randNum.Next().ToString();
                debitos.anoDocumento = "2023";
                //numProcesso = "1250";
                debitos.gerencia = "GGTAB";
                debitos.nomePessoa = "JOSEMILDA B. C. ALBUQUERQUE";
                debitos.receita = "";
                debitos.unidadeArrecadadora = 10707;
                //dataMulta = DateTime.Now.ToShortDateString();
                //valorMulta = "650";
                //dataVencimento = "28/11/2023";

                GravarMensagem($"******************** Início inserção do débito às {DateTime.Now} ********************");

                PessoaAutuadaDto pessoaAutuadaDto = new PessoaAutuadaDto();

                if (debitos.sistemaOrigem == SistemaOrigem.SEI.GetType().GetMember(SistemaOrigem.SEI.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description)
                {
                    debitos.tipoDebito = TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description;
                    debitos.receita = Receita.CobrancaMulta.GetType().GetMember(Receita.CobrancaMulta.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description;
                    debitos.unidadeArrecadadora = (int)UnidadeArrecadadora.DistritoFederal;
                }

                GravarMensagem("cpf_cnpj: " + debitos.cpf_cnpj);
                GravarMensagem("sistemaOrigem: " + debitos.sistemaOrigem);
                GravarMensagem("tipoDebito: " + debitos.tipoDebito);
                GravarMensagem("numDocumento: " + debitos.numDocumento);
                GravarMensagem("anoDocumento: " + debitos.anoDocumento);
                GravarMensagem("numProcesso: " + debitos.numProcesso);
                GravarMensagem("gerencia: " + debitos.gerencia);
                GravarMensagem("nomePessoa: " + debitos.nomePessoa);
                GravarMensagem("receita: " + debitos.receita);
                GravarMensagem("unidadeArrecadadora: " + debitos.unidadeArrecadadora);
                GravarMensagem("dataMulta: " + debitos.dataMulta);
                GravarMensagem("valorMulta: " + debitos.valorMulta);
                GravarMensagem("dataVencimento: " + debitos.dataVencimento);

                if (debitos.cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica)
                    pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaFisicaBaseDbCorporativo(debitos.cpf_cnpj);
                else
                    pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaJuridicaBaseDbCorporativo(debitos.cpf_cnpj);

                if (pessoaAutuadaDto == null)
                    return incluirDebitoResponseDto;

                if (!_pessoaAutuadaService.VerificarExistenciaPessoaAutuada(debitos.cpf_cnpj))
                {
                    if (!_pessoaAutuadaService.IncluirPessoaAutuada(debitos.cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
                        return incluirDebitoResponseDto;
                }
                else
                {
                    if (!_pessoaAutuadaService.AtualizarPessoaAutuada(debitos.cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
                        return incluirDebitoResponseDto;
                }

                if (_debitoService.VerificaSeDebitoEstaCadastrado(debitos.tipoDebito, debitos.numDocumento, debitos.anoDocumento, debitos.unidadeArrecadadora))
                    return incluirDebitoResponseDto;

                GravarMensagem("Inserindo um novo débito...");

                if (!_debitoService.IncluirDebito(debitos.cpf_cnpj, debitos.tipoDebito, debitos.numDocumento, debitos.anoDocumento, debitos.numProcesso, debitos.gerencia, debitos.nomePessoa, debitos.receita, debitos.unidadeArrecadadora, debitos.dataMulta, debitos.valorMulta))
                    return incluirDebitoResponseDto;

                GravarMensagem("Débito inserido com sucesso!");

                var codigoDebito = _debitoService.ObterCodigoDebito(debitos.tipoDebito, debitos.numDocumento, debitos.anoDocumento, debitos.unidadeArrecadadora);

                if (codigoDebito == 0)
                    return incluirDebitoResponseDto;

                GravarMensagem($"Código do débito: {codigoDebito}");

                if (!_debitoService.IncluirHistoricoSituacaoDebito(codigoDebito, debitos.tipoDebito, debitos.numDocumento, debitos.anoDocumento, debitos.unidadeArrecadadora))
                    return incluirDebitoResponseDto;

                GravarMensagem("Realizando notificação do débito...");

                var retorno = _debitoService.GerarNotificacaoDebito(codigoDebito, debitos.valorMulta, "01/01/2099", "0", "0", "0", "0", debitos.cpf_cnpj);

                if (retorno.sucesso)
                {
                    GravarMensagem("Débito notificado com sucesso!");

                    GravarMensagem("Atualizando situação do débito...");

                    if (_debitoService.AtualizarSituacaoDebito(codigoDebito, 3))
                        _debitoService.IncluirHistoricoSituacaoDebito(codigoDebito, 3, "PAULO.ALBUQUERQUE");

                    GravarMensagem("Débito atualizado com sucesso!");
                }

                GravarMensagem("Inserindo parcela do débito...");

                if (!_debitoService.IncluirParcelaDebito(codigoDebito, retorno.nossoNumero, "01/01/2099", debitos.valorMulta))
                    return incluirDebitoResponseDto;

                if (!_debitoService.AtualizaNossoNumero(retorno.codigoBoletoRegistrado, retorno.nossoNumero))
                    return incluirDebitoResponseDto;

                GravarMensagem("Parcela do débito inserida com sucesso!");

                GravarMensagem($"******************** Fim inserção do débito às {DateTime.Now} ********************");

                incluirDebitoResponseDto.CodigoReceita = 10;
                incluirDebitoResponseDto.IdContrato = 100;
                incluirDebitoResponseDto.LinkBoleto = $"https://unigru-pre.anvisa.gov.br/unigru/guia/2023/200540";
                incluirDebitoResponseDto.NumeroFistel = "50";
                incluirDebitoResponseDto.NumeroSequencial = 20;

                return incluirDebitoResponseDto;
            }
            catch (Exception ex)
            {
                GravarMensagem($"Erro: {ex.Message}");

                return incluirDebitoResponseDto;
            }
        }

        public void GravarMensagem(string mensagem)
        {
            var path = HttpContext.Current.Server.MapPath("/log/");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString() + ".txt";

            StreamWriter sw = new StreamWriter(path, true);

            sw.WriteLine(DateTime.Now.ToString() + " " + mensagem);

            sw.Close();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public XmlDocument GetWSDL()
        {
            string nameElement = string.Empty;
            bool elementIsEmpty = false;
            bool excluirTagElement = false;
            bool isAddress = false;
            bool isSchema = false;

            var urlWSDL = ConfigurationManager.AppSettings["UrlWSDL"];
            var urlWS = ConfigurationManager.AppSettings["UrlWS"];

            StringBuilder sb = new StringBuilder();

            WebClient client = new WebClient();

            var wsdlOriginal = client.DownloadString(urlWSDL);

            byte[] byteArray = Encoding.UTF8.GetBytes(wsdlOriginal);

            MemoryStream stream = new MemoryStream(byteArray);

            XmlTextReader reader = new XmlTextReader(stream);

            while (reader.Read())
            {
                if (reader.Name == "s:schema" && reader.NodeType == XmlNodeType.Element)
                    isSchema = true;  
                
                elementIsEmpty = reader.IsEmptyElement;

                excluirTagElement = reader.Name == "s:element" && !elementIsEmpty ? true : false;

                isAddress = reader.Name.Contains("address") ? true : false;

                if (excluirTagElement)
                {
                    nameElement = reader.GetAttribute("name");

                    continue;
                }

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:

                        sb.Append("<" + reader.Name);

                        if (!string.IsNullOrEmpty(nameElement))
                        {
                            sb.Append($" name='{nameElement}'");

                            nameElement = string.Empty;
                        }

                        while (reader.MoveToNextAttribute())
                        {
                            //if (reader.Name.Equals("soapAction"))
                            //    sb.Append(" " + reader.Name + "='" + reader.Value.Replace("http://tempuri.org/", "") + "'");
                            //else if (reader.Name.Equals("xmlns:tns") || reader.Name.Equals("targetNamespace"))
                            //    sb.Append(" " + reader.Name + "='" + "http://10.103.0.41:8083/WSCodivaService.asmx" + "'");
                            //else
                            //    sb.Append(" " + reader.Name + "='" + reader.Value + "'");

                            //if (reader.Name.Equals("soapAction"))
                            //    sb.Append(" " + reader.Name + "='" + reader.Value.Replace("http://tempuri.org/", "http://10.103.0.41:8083/WSCodivaService.asmx?op=") + "'");
                            //else if (reader.Name.Equals("xmlns:tns") || reader.Name.Equals("targetNamespace"))
                            //    sb.Append(" " + reader.Name + "='" + "http://10.103.0.41:8083/WSCodivaService.asmx?op=" + "'");
                            //else
                            //    sb.Append(" " + reader.Name + "='" + reader.Value + "'");

                            sb.Append(" " + reader.Name + "='" + reader.Value + "'");
                        }

                        sb.AppendLine(elementIsEmpty ? "/>" : ">");

                        if (isSchema)
                        {
                            sb.Append("<s:element name=\"IncluirDebito\" type=\"tns:IncluirDebito\"/>");

                            isSchema = false;
                        }

                        break;

                    case XmlNodeType.EndElement:

                        sb.Append("</" + reader.Name);
                        sb.AppendLine(">");

                        break;
                }
            }

            var wsdlModificado = sb.ToString();



            XmlDocument xml = new XmlDocument();

            xml.LoadXml(wsdlModificado);

            return xml;
        }
    }
}
