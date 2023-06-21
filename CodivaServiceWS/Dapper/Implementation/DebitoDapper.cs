using CodivaServiceWS.Connection;
using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Model;
using Dapper;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class DebitoDapper : IDebitoDapper
    {
        public bool IncluirDebito(int codPessoaDevedora, string receita, int codStatusDebito, int unidadeArrecadadora, string anoDocumento, string numDocumento, string numProcesso, int tipoDebito, string valorMulta)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                string sql = $@"INSERT INTO DBCODIVA.TB_DEBITO
                                (
                                    CO_PESSOA_DEVEDORA,
                                    CO_RECEITA,                                        
                                    CO_STATUS_DEBITO,
                                    CO_UNIDADE_CONVENIO,                                       
                                    NU_ANO_DOCUMENTO,
                                    NU_DOCUMENTO,
                                    NU_PROCESSO,
                                    ST_QUITADO_CONAU,
                                    TP_DEBITO,
                                    VL_ORIGINAL,
                                    VL_SALDO,
                                    DT_ALTERACAO,
                                    DT_VENCIMENTO
                                )
                                VALUES
                                (
                                    {codPessoaDevedora},
                                    '{receita}',
                                    1,
                                    {unidadeArrecadadora},
                                    {anoDocumento.Substring(2, 2)},
                                    {numDocumento},
                                    {numProcesso},
                                    'N',
                                    {tipoDebito},
                                    {valorMulta},
                                    500,
                                    to_date('{DateTime.Now}', 'dd/mm/yyyy HH24:mi:ss'),
                                    to_date('01/01/2999', 'dd/mm/yyyy HH24:mi:ss')
                                )";

                var result = connection.Execute(sql);

                return result > 0;
            }
        }

        public bool VerificaSeDebitoEstaCadastrado(string tipoDebito, string numDocumento, string anoDocumento, int unidadeArrecadadora)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var codTipoDebito = tipoDebito.ToLower() == TipoDebito.AutoInfracao.GetType().GetMember(TipoDebito.AutoInfracao.ToString()).First().GetCustomAttribute<DescriptionAttribute>().Description.ToLower() ? 1 : 14;

                var result = connection.QueryFirstOrDefault<TBPessoaDevedora>($"SELECT CO_SEQ_DEBITO, NU_DOCUMENTO, TP_DEBITO FROM DBCODIVA.TB_DEBITO WHERE TP_DEBITO = {codTipoDebito} AND NU_DOCUMENTO = '{numDocumento}' AND NU_ANO_DOCUMENTO = '{anoDocumento}' AND CO_UNIDADE_CONVENIO = {unidadeArrecadadora}");

                return result != null;
            }
        }

        public bool IncluirHistoricoSituacaoDebito(int codDebito, int coStatusDebito, string coUsuario)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                string sql = $@"INSERT INTO DBCODIVA.TH_SITUACAO
                                (
                                    CO_DEBITO,    
                                    CO_STATUS_DEBITO,
                                    CO_USUARIO,
                                    DT_SITUACAO,
                                    DT_ALTERACAO
                                )
                                VALUES
                                (
                                    {codDebito},
                                    {coStatusDebito},
                                    '{coUsuario}',
                                    to_date('{DateTime.Now}', 'dd/mm/yyyy HH24:mi:ss'),
                                    to_date('{DateTime.Now}', 'dd/mm/yyyy HH24:mi:ss')
                                )";

                var result = connection.Execute(sql);

                return result > 0;
            }
        }
    }
}
