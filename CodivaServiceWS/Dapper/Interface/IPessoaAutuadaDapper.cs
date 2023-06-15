using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dapper.Interface
{
    public interface IPessoaAutuadaDapper
    {
        bool VerificarExistenciaPessoaAutuada(string cpf, string nome);
        bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio);
        bool AlterarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio);
        int ObterCodigoPessoaAutuada(string cpf);
    }
}
