using CodivaServiceWS.Connection;
using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class BaseDapper : IBaseDapper
    {
        public int ObterCodigoCidadePorNome(string nomeCidade)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.ExecuteScalar<int>($"SELECT CO_SEQ_CIDADE FROM DBSISTRU.TB_CIDADE WHERE NO_CIDADE = '{nomeCidade}'");

                return result;
            }
        }

        public int ObterCodigoPessoaDevedoraPorCpfCnpj(string cpf_cnpj)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.ExecuteScalar<int>($"SELECT CO_SEQ_PESSOA_DEVEDORA FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE NU_CPF_CNPJ = '{cpf_cnpj}'");

                return result;
            }
        }

        public int ObterCodigoDebito(int tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                string sql = $@"SELECT CO_SEQ_DEBITO
                                FROM DBCODIVA.TB_DEBITO
                                WHERE TP_DEBITO = {tipoDebito}
                                AND   NU_DOCUMENTO = '{numDocumento}'
                                AND   NU_ANO_DOCUMENTO = '{anoDocumento.Substring(2,2)}'
                                AND   CO_UNIDADE_CONVENIO = {unidadeArrecadadora}";

                var result = connection.ExecuteScalar<int>(sql);

                return result;
            }
        }
    }
}
