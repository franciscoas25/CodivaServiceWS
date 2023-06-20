using CodivaServiceWS.Connection;
using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Dto;
using CodivaServiceWS.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class PessoaAutuadaDapper : BaseDapper, IPessoaAutuadaDapper
    {
        //public bool VerificarExistenciaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial)
        //{
        //    using (IDbConnection connection = CodivaServiceConnection.GetConnection())
        //    {
        //        var result = connection.QueryFirstOrDefault<TBPessoaFisica>($"SELECT * FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE NU_CPF_CNPJ = '{cpf_cnpj}' OR NO_PESSOA = '{nome_razaoSocial}'");

        //        return result != null;
        //    }
        //}

        public bool VerificarExistenciaPessoaAutuada(string cpf_cnpj)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.QueryFirstOrDefault<TBPessoaFisica>($"SELECT * FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE NU_CPF_CNPJ = '{cpf_cnpj}'");

                return result != null;
            }
        }

        public PessoaAutuadaDto ObterDadosPessoaFisicaBaseDbCorporativo(string cpf_cnpj)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var dadosPessoaFisica = connection.QueryFirstOrDefault<PessoaAutuadaDto>($@"SELECT PF.NO_PESSOA_FISICA AS NOME_RAZAOSOCIAL,
                                                                                                  P.DS_ENDERECO AS ENDERECO,
                                                                                                  P.NU_CEP AS CEP,
                                                                                                  PF.NO_CIDADE_NATURALIDADE AS CIDADE
                                                                                           FROM   DBCORPORATIVO.TB_PESSOA_FISICA PF,
                                                                                                  DBCORPORATIVO.TB_PESSOA P
                                                                                           WHERE  PF.ID_PESSOA_FISICA = P.ID_PESSOA 
                                                                                           AND    NU_CPF = '{cpf_cnpj}'");

                return dadosPessoaFisica;
            }
        }

        public PessoaAutuadaDto ObterDadosPessoaJuridicaBaseDbCorporativo(string cpf_cnpj)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var dadosPessoaJuridica = connection.QueryFirstOrDefault<PessoaAutuadaDto>($@"SELECT PJ.NO_RAZAO_SOCIAL AS NOME_RAZAOSOCIAL,
                                                                                                       P.DS_ENDERECO AS ENDERECO,
                                                                                                       P.NU_CEP AS CEP,
                                                                                                       C.NO_CIDADE AS CIDADE
                                                                                                FROM   DBCORPORATIVO.TB_PESSOA_JURIDICA PJ,
                                                                                                       DBCORPORATIVO.TB_PESSOA P,
                                                                                                       DBGERAL.TB_CIDADE C
                                                                                                WHERE  PJ.ID_PESSOA_JURIDICA = P.ID_PESSOA
                                                                                                AND    P.CO_CIDADE = C.CO_SEQ_CIDADE
                                                                                                AND    PJ.NU_CNPJ = '{cpf_cnpj}'");

                return dadosPessoaJuridica;
            }
        }

        public bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            var tipoPessoa = cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica ? 'N' : 'S';

            int codigoCidade = ObterCodigoCidadePorNome(municipio);

            string sql = $@"INSERT INTO DBCODIVA.TB_PESSOA_DEVEDORA 
                            (
                                TP_PESSOA,
                                NO_PESSOA,
                                NU_CPF_CNPJ,
                                DS_ENDERECO_PESSOA,
                                NU_CEP_PESSOA,
                                CO_CIDADE,
                                DT_ALTERACAO,
                            ) 
                            VALUES 
                            (
                                '{tipoPessoa}',
                                '{nome_razaoSocial}',
                                '{cpf_cnpj}',
                                '{endereco}',
                                '{cep}',
                                '{codigoCidade}',
                                '{DateTime.Now}'
                            );";

            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.Execute(sql);

                return result > 0;
            }
        }

        public bool AtualizarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            var tipoPessoa = cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica ? 'N' : 'S';

            int codigoCidade = ObterCodigoCidadePorNome(municipio);

            int coSeqPessoaDevedora = ObterCodigoPessoaAutuada(cpf_cnpj);

            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                string sql = $@"UPDATE DBCODIVA.TB_PESSOA_DEVEDORA 
                            SET TP_PESSOA = '{tipoPessoa}',
                                NO_PESSOA = '{nome_razaoSocial}',
                                DS_ENDERECO_PESSOA = '{endereco}',
                                NU_CEP_PESSOA = '{cep}',
                                CO_CIDADE = '{codigoCidade}',
                                DT_ALTERACAO = {DateTime.Now}
                            WHERE CO_SEQ_PESSOA_DEVEDORA = {coSeqPessoaDevedora}";

                var result = connection.Execute(sql);

                return result > 0;
            }
        }

        public DateTime ObterUltimaDataAlteracaoPessoaAutuada(string cpf_cnpj)
        {
            int coSeqPessoaDevedora = ObterCodigoPessoaAutuada(cpf_cnpj);

            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                string sql = $"SELECT DT_ALTERACAO FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE CO_SEQ_PESSOA_DEVEDORA = {coSeqPessoaDevedora}";

                return (DateTime)connection.ExecuteScalar(sql);
            }
        }

        public int ObterCodigoPessoaAutuada(string cpf)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.ExecuteScalar<int>($"SELECT CO_SEQ_PESSOA_DEVEDORA FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE NU_CPF_CNPJ = {cpf}");

                return result;
            }
        }
    }
}
