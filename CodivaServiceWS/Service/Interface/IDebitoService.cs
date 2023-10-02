using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Service.Interface
{
    public interface IDebitoService
    {
        bool IncluirDebito(string cpf_cnpj, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta);
        bool ValidaDebito(string cpf_cnpj, string tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, string nomePessoa, string receita, int unidadeArrecadadora, string dataMulta, string valorMulta);
        bool VerificaSeDebitoEstaCadastrado(string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora);
        bool IncluirHistoricoSituacaoDebito(int codigoDebito, string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora);
        decimal CalculaNossoNumero(string uf, string receita, string tipoNossoNumero);
        string GerarNotificacaoDebito(int codigoDebito, string nossoNumero, string valorMulta, string dataVencimento, string percentualSelic, string percentualMulta, string valorSelic, string valorMultaSelic);
        int ObterCodigoDebito(string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora);
        bool AlterarDebito(int codigoDebito, string anoDocumento, string numDocumento, string numProcesso, string valorMulta, string dataVencimento);
        bool IncluirParcelaDebito(int codigoDebito, string nossoNumero, string dataVencimento, string valorMulta);
        bool AtualizarSituacaoDebito(int codigoDebito, int codigoSituacao);
    }
}
