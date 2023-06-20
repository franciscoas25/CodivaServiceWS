using CodivaServiceWS.Connection;
using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Model;
using Dapper;
using System.Data;
using System.Text;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class DebitoDapper : IDebitoDapper
    {
        public bool IncluirDebito(string query)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.Execute(query);

                return result > 0;
            }
        }

        public bool VerificaSeDebitoEstaCadastrado(int tipoDebito, string numDocumento, string anoDocumento, string unidadeArrecadadora)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.QueryFirstOrDefault<TBPessoaDevedora>($"SELECT CO_SEQ_DEBITO, NU_DOCUMENTO, TP_DEBITO FROM DBCODIVA.TB_DEBITO WHERE TP_DEBITO = {tipoDebito} AND AND NU_DOCUMENTO = '{numDocumento}' AND NU_ANO_DOCUMENTO = '{anoDocumento}' AND CO_UNIDADE_CONVENIO = '{unidadeArrecadadora}'");

                return result != null;
            }
        }
    }
}
