using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dapper.Interface
{
    public interface IBaseDapper
    {
        int ObterCodigoCidadePorNome(string nomeCidade);
        int ObterCodigoPessoaDevedoraPorCpfCnpj(string cpf_cnpj);
    }
}
