using CodivaServiceWS.Dto;
using CodivaServiceWS.Service.Implementation;
using CodivaServiceWS.Service.Interface;
using Ninject;
using Ninject.Web;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Services;
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
        public bool IncluirDebito(string cpf_cnpj, string sistemaOrigem, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta)
        {
            try
            {
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
                    return false;

                if (!_pessoaAutuadaService.VerificarExistenciaPessoaAutuada(cpf_cnpj))
                {
                    if (!_pessoaAutuadaService.IncluirPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
                        return false;
                }
                else
                {
                    if (!_pessoaAutuadaService.AtualizarPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
                        return false;
                }

                if (_debitoService.VerificaSeDebitoEstaCadastrado(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora))
                    return false;

                //return _debitoService.IncluirDebito(cpf_cnpj, tipoDebito, numDocumento, anoDocumento, numProcesso, gerencia, nomePessoa, receita, unidadeArrecadadora, dataMulta, valorMulta);

                if (!_debitoService.IncluirDebito(cpf_cnpj, tipoDebito, numDocumento, anoDocumento, numProcesso, gerencia, nomePessoa, receita, unidadeArrecadadora, dataMulta, valorMulta))
                    return false;

                var codigoDebito = _debitoService.ObterCodigoDebito(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora);

                if (codigoDebito == 0)
                    return false;

                if (!_debitoService.IncluirHistoricoSituacaoDebito(codigoDebito, tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora))
                    return false;

                //Cálculo do Nosso Número
                //var nossoNumero = _debitoService.CalculaNossoNumero("11", receita, "0");

                //Notificação Administrativa
                _debitoService.GerarNotificacaoDebito(codigoDebito, "11012323000001820", valorMulta, "01/01/2099", "0", "0", "0", "0");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //[WebMethod]
        //public bool IncluirPessoaAutuada(string cpf_cnpj)
        //{
        //    try
        //    {
        //        PessoaAutuadaDto pessoaAutuadaDto = new PessoaAutuadaDto();

        //        if (string.IsNullOrEmpty(cpf_cnpj))
        //            return false;

        //        if (cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica)
        //            pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaFisicaBaseDbCorporativo(cpf_cnpj);
        //        else
        //            pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaJuridicaBaseDbCorporativo(cpf_cnpj);

        //        //if (!_pessoaAutuadaService.ValidaPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.CIDADE))
        //        //    return false;

        //        if (!_pessoaAutuadaService.VerificarExistenciaPessoaAutuada(cpf_cnpj))
        //        {
        //            if (_pessoaAutuadaService.IncluirPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE))
        //            {
        //                var coSeqPessoaDevedora = _pessoaAutuadaService.ObterCodigoPessoaAutuada(cpf_cnpj);

        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return _pessoaAutuadaService.AtualizarPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.NM_CIDADE, pessoaAutuadaDto.COD_CIDADE);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
    }
}
