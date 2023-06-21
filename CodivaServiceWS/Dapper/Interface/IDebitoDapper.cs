using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dapper.Interface
{
    public interface IDebitoDapper
    {
        bool IncluirDebito(int codPessoaDevedora, string receita, int codStatusDebito, int unidadeArrecadadora, string anoDocumento, string numDocumento, string numProcesso, int tipoDebito, string valorMulta);
        bool VerificaSeDebitoEstaCadastrado(string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora);
    }
}
