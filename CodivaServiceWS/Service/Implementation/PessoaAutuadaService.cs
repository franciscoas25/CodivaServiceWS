using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Dto;
using CodivaServiceWS.Service.Interface;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Service.Implementation
{
    public class PessoaAutuadaService : IPessoaAutuadaService
    {
        [Inject]
        public IPessoaAutuadaDapper _pessoaAutuadaDapper { get; set; }

        public bool VerificarExistenciaPessoaAutuada(string cpf_cnpj)
        {
            return _pessoaAutuadaDapper.VerificarExistenciaPessoaAutuada(cpf_cnpj);
        }

        public PessoaAutuadaDto ObterDadosPessoaFisicaBaseDbCorporativo(string cpf_cnpj)
        {
            return _pessoaAutuadaDapper.ObterDadosPessoaFisicaBaseDbCorporativo(cpf_cnpj);
        }

        public PessoaAutuadaDto ObterDadosPessoaJuridicaBaseDbCorporativo(string cpf_cnpj)
        {
            return _pessoaAutuadaDapper.ObterDadosPessoaJuridicaBaseDbCorporativo(cpf_cnpj);
        }

        public bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string nmCidade, int codCidade)
        {
            return _pessoaAutuadaDapper.IncluirPessoaAutuada(cpf_cnpj, nome_razaoSocial, endereco, cep, nmCidade, codCidade);
        }

        public bool AtualizarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string nmCidade, int codCidade)
        {
            var ultimaDataAlteracaoPessoaAutuada = _pessoaAutuadaDapper.ObterUltimaDataAlteracaoPessoaAutuada(cpf_cnpj);

            if (ultimaDataAlteracaoPessoaAutuada.Month != DateTime.Now.Month)
            {
                return _pessoaAutuadaDapper.AtualizarPessoaAutuada(cpf_cnpj, nome_razaoSocial, endereco, cep, nmCidade, codCidade);
            }

            return true;
        }

        public int ObterCodigoPessoaAutuada(string cpf_cnpj)
        {
            return _pessoaAutuadaDapper.ObterCodigoPessoaAutuada(cpf_cnpj);
        }

        public bool ValidaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            return !string.IsNullOrEmpty(cpf_cnpj) && !string.IsNullOrEmpty(nome_razaoSocial) && !string.IsNullOrEmpty(endereco) && !string.IsNullOrEmpty(cep) && !string.IsNullOrEmpty(municipio);
        }
    }
}
