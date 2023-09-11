using CodivaServiceWS.Dto;
using CodivaServiceWS.Service.Implementation;
using CodivaServiceWS.Service.Interface;
using Ninject;
using Ninject.Web;
using System;
using System.ComponentModel;
using System.Data.Entity.Core.Mapping;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class WSCodivaService : WebServiceBase    
    {
        [Inject]
        public IPessoaAutuadaService _pessoaAutuadaService { get; set; }
        [Inject]
        public IDebitoService _debitoService { get; set; }

        [WebMethod]
        public IncluirDebitoResponseDto IncluirDebito(string cpf_cnpj, string sistemaOrigem, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta, string dataVencimento)
        {
            IncluirDebitoResponseDto incluirDebitoResponseDto = new IncluirDebitoResponseDto();

            try
            {
                GravarMensagem("Inserindo um novo débito...");
                
                PessoaAutuadaDto pessoaAutuadaDto = new PessoaAutuadaDto();

                if (sistemaOrigem == SistemaOrigem.SEI.GetType().GetMember(SistemaOrigem.SEI.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description)
                {
                    tipoDebito = TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description;
                    receita = Receita.CobrancaMulta.GetType().GetMember(Receita.CobrancaMulta.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description;
                    unidadeArrecadadora = (int)UnidadeArrecadadora.DistritoFederal;
                }

                if (cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica)
                    pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaFisicaBaseDbCorporativo(cpf_cnpj);
                else
                    pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaJuridicaBaseDbCorporativo(cpf_cnpj);

                if (pessoaAutuadaDto == null)
                    return incluirDebitoResponseDto;

                if (!_pessoaAutuadaService.VerificarExistenciaPessoaAutuada(cpf_cnpj))
                {
                    if (!_pessoaAutuadaService.IncluirPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
                        return incluirDebitoResponseDto;
                }
                else
                {
                    if (!_pessoaAutuadaService.AtualizarPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
                        return incluirDebitoResponseDto;
                }

                if (_debitoService.VerificaSeDebitoEstaCadastrado(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora))
                    return incluirDebitoResponseDto;

                if (!_debitoService.IncluirDebito(cpf_cnpj, tipoDebito, numDocumento, anoDocumento, numProcesso, gerencia, nomePessoa, receita, unidadeArrecadadora, dataMulta, valorMulta))
                    return incluirDebitoResponseDto;

                var codigoDebito = _debitoService.ObterCodigoDebito(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora);

                if (codigoDebito == 0)
                    return incluirDebitoResponseDto;

                if (!_debitoService.IncluirHistoricoSituacaoDebito(codigoDebito, tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora))
                    return incluirDebitoResponseDto;

                //var nossoNumero = _debitoService.CalculaNossoNumero("11", receita, "0");

                //Notificação Administrativa
                _debitoService.GerarNotificacaoDebito(codigoDebito, "11012323000001820", valorMulta, "01/01/2099", "0", "0", "0", "0");

                incluirDebitoResponseDto.CodigoReceita = 10;
                incluirDebitoResponseDto.IdContrato = 100;
                incluirDebitoResponseDto.LinkBoleto = "https://unigru-pre.anvisa.gov.br/unigru/guia/2023/739895";
                incluirDebitoResponseDto.NumeroFistel = "50";
                incluirDebitoResponseDto.NumeroSequencial = 20;

                return incluirDebitoResponseDto;
            }
            catch (Exception ex)
            {
                return incluirDebitoResponseDto;
            }
        }

        //[WebMethod]
        //public bool AtualizarDebito(int codigoDebito, string anoDocumento, string numDocumento, string numProcesso, string valorMulta, string dataVencimento)
        //{
        //    return _debitoService.AlterarDebito(codigoDebito, anoDocumento, numDocumento, numProcesso, valorMulta, dataVencimento);
        //}

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
