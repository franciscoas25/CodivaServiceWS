using CodivaServiceWS.Dapper.Interface;
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

        public bool VerificarExistenciaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial)
        {
            return _pessoaAutuadaDapper.VerificarExistenciaPessoaAutuada(cpf_cnpj, nome_razaoSocial);
        }

        public bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            return _pessoaAutuadaDapper.IncluirPessoaAutuada(cpf_cnpj, nome_razaoSocial, endereco, cep, municipio);
        }

        public bool AlterarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            return _pessoaAutuadaDapper.AlterarPessoaAutuada(cpf_cnpj, nome_razaoSocial, endereco, cep, municipio);
        }

        public int ObterCodigoPessoaAutuada(string cpf)
        {
            return _pessoaAutuadaDapper.ObterCodigoPessoaAutuada(cpf);
        }

        public bool ValidaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            return !string.IsNullOrEmpty(cpf_cnpj) && !string.IsNullOrEmpty(nome_razaoSocial) && !string.IsNullOrEmpty(endereco) && !string.IsNullOrEmpty(cep) && !string.IsNullOrEmpty(municipio);
        }
    }
}
