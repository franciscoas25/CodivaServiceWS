using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Service.Interface
{
    public interface IPessoaAutuadaService
    {
        bool VerificarExistenciaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial);
        bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio);
        bool AlterarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio);
        int ObterCodigoPessoaAutuada(string cpf);
        bool ValidaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio);
    }
}
