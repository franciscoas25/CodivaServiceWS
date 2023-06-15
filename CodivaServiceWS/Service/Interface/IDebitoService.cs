using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Service.Interface
{
    public interface IDebitoService
    {
        bool IncluirDebito(int tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, int codigoDecisao, string nomePessoa, string receita, string unidadeArrecadadora, string dataInicialAtualizacaoMonetaria, string valorInicialAtualizacaoMonetaria, string dataVencimento, string banco, string agencia, string conta);
        bool ValidaDebito(int tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, int codigoDecisao, string nomePessoa, string receita, string unidadeArrecadadora, string dataInicialAtualizacaoMonetaria, string valorInicialAtualizacaoMonetaria, string dataVencimento, string banco, string agencia, string conta);
        bool VerificaSeDebitoEstaCadastrado(int tipoDebito, string numDocumento, string anoDocumento, string unidadeArrecadadora);
    }
}
