using CodivaServiceWS.Dto;
using CodivaServiceWS.Service.Implementation;
using CodivaServiceWS.Service.Interface;
using Ninject;
using Ninject.Web;
using System;
using System.Web.Services;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS
{
    /// <summary>
    /// Descrição resumida de WSCodivaService
    /// </summary>
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
        public bool IncluirDebito(int tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, int codigoDecisao, string nomePessoa, string receita, string unidadeArrecadadora, string dataInicialAtualizacaoMonetaria, string valorInicialAtualizacaoMonetaria, string dataVencimento, string banco, string agencia, string conta)
        {
            try
            {
                if (!_debitoService.ValidaDebito(tipoDebito, numDocumento, anoDocumento, numProcesso, gerencia, codigoDecisao, nomePessoa, receita, unidadeArrecadadora, dataInicialAtualizacaoMonetaria, valorInicialAtualizacaoMonetaria, dataVencimento, banco, agencia, conta))
                    return false;

                if (_debitoService.VerificaSeDebitoEstaCadastrado(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora))
                    return false;

                return _debitoService.IncluirDebito(tipoDebito, numDocumento, anoDocumento, numProcesso, gerencia, codigoDecisao, nomePessoa, receita, unidadeArrecadadora, dataInicialAtualizacaoMonetaria, valorInicialAtualizacaoMonetaria, dataVencimento, banco, agencia, conta);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [WebMethod]
        public bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            try
            {
                PessoaAutuadaDto pessoaAutuadaDto = new PessoaAutuadaDto();

                if (string.IsNullOrEmpty(cpf_cnpj))
                    return false;

                if (cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica)
                    pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaFisicaBaseDbCorporativo(cpf_cnpj);
                else
                    pessoaAutuadaDto = _pessoaAutuadaService.ObterDadosPessoaJuridicaBaseDbCorporativo(cpf_cnpj);

                if (!_pessoaAutuadaService.ValidaPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.CIDADE))
                    return false;

                if (!_pessoaAutuadaService.VerificarExistenciaPessoaAutuada(cpf_cnpj))
                {
                    if (_pessoaAutuadaService.IncluirPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.CIDADE))
                    {
                        var coSeqPessoaDevedora = _pessoaAutuadaService.ObterCodigoPessoaAutuada(cpf_cnpj);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return _pessoaAutuadaService.AtualizarPessoaAutuada(cpf_cnpj, pessoaAutuadaDto.NOME_RAZAOSOCIAL, pessoaAutuadaDto.ENDERECO, pessoaAutuadaDto.CEP, pessoaAutuadaDto.CIDADE);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
