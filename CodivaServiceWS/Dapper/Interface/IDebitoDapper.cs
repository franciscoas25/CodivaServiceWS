using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dapper.Interface
{
    public interface IDebitoDapper
    {
        bool IncluirDebito(string query);
        bool VerificaSeDebitoEstaCadastrado(int tipoDebito, string numDocumento, string anoDocumento, string unidadeArrecadadora);
    }
}
