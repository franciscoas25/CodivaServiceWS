using CodivaServiceWS.Connection;
using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Model;
using Dapper;
using Dapper.Oracle;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class DebitoDapper : IDebitoDapper
    {
        public bool IncluirDebito(int codPessoaDevedora, string receita, int codStatusDebito, int unidadeArrecadadora, string anoDocumento, string numDocumento, string numProcesso, int tipoDebito, string dataMulta, string valorMulta, string dataVencimento)
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
                                    VL_DESCONTO,
                                    DT_ALTERACAO,
                                    DT_VENCIMENTO, 
                                    DT_INICIAL,
                                    ST_DEBITO_EXIGIVEL
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
                                    0,
                                    0,
                                    to_date('{DateTime.Now}', 'dd/mm/yyyy HH24:mi:ss'),
                                    null,
                                    to_date('{dataMulta}', 'dd/mm/yyyy HH24:mi:ss'),
                                    'N'
                                )";

                var result = connection.Execute(sql);

                return result > 0;
            }
        }

        public bool AlterarDebito(int codigoDebito, DadosDebito dadosDebito)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                StringBuilder query = new StringBuilder();

                query.Append("UPDATE DBCODIVA.TB_DEBITO SET ");

                if (!string.IsNullOrEmpty(dadosDebito.AnoDocumento))
                    query.Append($"NU_ANO_DOCUMENTO = '{dadosDebito.AnoDocumento}',");

                if (!string.IsNullOrEmpty(dadosDebito.NumDocumento))
                    query.Append($"NU_DOCUMENTO = '{dadosDebito.NumDocumento}',");

                if (!string.IsNullOrEmpty(dadosDebito.NumProcesso))
                    query.Append($"NU_PROCESSO = '{dadosDebito.NumProcesso}',");

                if (!string.IsNullOrEmpty(dadosDebito.ValorDebito))
                    query.Append($"VL_ORIGINAL = '{dadosDebito.ValorDebito}',");

                if (!string.IsNullOrEmpty(dadosDebito.DataVencimento.ToString()))
                {
                    if (DateTime.TryParse(dadosDebito.DataVencimento, out DateTime dataVencimentoResult))
                        query.Append($"DT_VENCIMENTO = '{dataVencimentoResult.ToShortDateString()}',");
                }

                query.Append($"DT_ALTERACAO = '{DateTime.Now.ToShortDateString()}' ");

                query.Append($"WHERE CO_SEQ_DEBITO = {codigoDebito}");

                var result = connection.Execute(query.ToString());

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
            using (IDbConnection connection = CodivaServiceConnection.GetConnection2())
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
                                )" ;

                var result = connection.Execute(sql);

                return result > 0;
            }
        }

        public decimal CalculaNossoNumero(string uf, string receita, string tipoNossoNumero)
        {
            string _connectionString = ConfigurationManager.ConnectionStrings["CodivaServiceConnectioString"].ConnectionString;

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@sUF", uf);
                parameters.Add("@sRec", receita);
                parameters.Add("@sTipNossoNum", tipoNossoNumero);
                parameters.Add("@sProxNossoNum", dbType: DbType.Decimal, direction: ParameterDirection.Output);

                connection.Execute("DBCODIVA.CALCULANOSSONUM", parameters, commandType: CommandType.StoredProcedure);
                
                return parameters.Get<decimal>("@sProxNossoNum");
            }
        }

        public DadosDebito ObterDadosDebito(int coDebito)
        {
            string sql = $@"SELECT
                            pes.no_pessoa as NomePessoa,
                            pes.nu_cep_pessoa as Cep,
                            pes.nu_cpf_cnpj as CpfCnpj,
                            pes.ds_endereco_pessoa as Endereco,
                            pes.ds_bairro_pessoa as Bairro,
                            deb.nu_processo as NumProcesso,
                            mun.no_cidade as Cidade,
                            mun.co_uf as UF,
                            deb.nu_ais_nacional,
                            deb.tp_debito,
                            tp.sg_debito as SiglaDebito,
                            deb.nu_ano_documento as AnoDocumento,
                            deb.nu_documento as NumDocumento,
                            deb.vl_original as ValorDebito,
                            deb.dt_vencimento as DataVencimento
                        FROM
                            dbcodiva.tb_debito          deb,
                            dbcodiva.tb_tipo_debito     tp,
                            dbcodiva.tb_pessoa_devedora pes,
                            dbsistru.tb_cidade          mun
                        WHERE
                                co_seq_debito = {coDebito}
                            AND deb.co_pessoa_devedora = pes.co_seq_pessoa_devedora
                            AND tp.co_tipo_debito = deb.tp_debito
                            AND pes.co_cidade = mun.co_seq_cidade";

            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.QueryFirstOrDefault<DadosDebito>(sql);

                return result;
            }
        }

        public bool IncluirParcelaDebito(int codigoDebito, string nossoNumero, string dataVencimento, string valorMulta)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection2())
            {
                string sql = $@"INSERT INTO DBCODIVA.TB_PARCELA
                                (
                                    CO_DEBITO,    
                                    NU_PARCELAMENTO,
                                    NU_PARCELA,
                                    CO_STATUS_PARCELA,
                                    NU_NOSSO_NUMERO,
                                    DT_VENCIMENTO,
                                    VL_PARCELA,
                                    NU_ORDEM_PARCELA,
                                    DT_ALTERACAO
                                )
                                VALUES
                                (
                                    {codigoDebito},
                                    0,
                                    1,
                                    0,
                                    '{nossoNumero}',
                                    to_date('{dataVencimento}', 'dd/mm/yyyy HH24:mi:ss'),
                                    {valorMulta},
                                    0,
                                    to_date('{DateTime.Now}', 'dd/mm/yyyy HH24:mi:ss')
                                )";
                
                var result = connection.Execute(sql);

                return result > 0;
            }
        }

        public bool AtualizarSituacaoDebito(int codigoDebito, int codigoSituacao)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection2())
            {
                string sql = $@"UPDATE DBCODIVA.TB_DEBITO
                                SET CO_STATUS_DEBITO = {codigoSituacao} 
                                WHERE CO_SEQ_DEBITO = {codigoDebito} ";

                var result = connection.Execute(sql);

                return result > 0;
            }
        }
    }
}
