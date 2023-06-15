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
using static CodivaServiceWS.Enum.Enum;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class PessoaAutuadaDapper : BaseDapper, IPessoaAutuadaDapper
    {
        public bool VerificarExistenciaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial)
        {
            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.QueryFirstOrDefault<TBPessoaFisica>($"SELECT * FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE NU_CPF_CNPJ = '{cpf_cnpj}' OR NO_PESSOA = '{nome_razaoSocial}'");

                return result != null;
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

        public bool AlterarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio)
        {
            var tipoPessoa = cpf_cnpj.Length == (int)TipoPessoa.PessoaFisica ? 'N' : 'S';

            int codigoCidade = ObterCodigoCidadePorNome(municipio);

            int coSeqPessoaDevedora = ObterCodigoPessoaAutuada(cpf_cnpj);

            string sql = $@"UPDATE DBCODIVA.TB_PESSOA_DEVEDORA 
                            SET TP_PESSOA = '{tipoPessoa}',
                                NO_PESSOA = '{nome_razaoSocial}',
                                DS_ENDERECO_PESSOA = '{endereco}',
                                NU_CEP_PESSOA = '{cep}',
                                CO_CIDADE = '{codigoCidade}',
                                DT_ALTERACAO = {DateTime.Now}
                            WHERE CO_SEQ_PESSOA_DEVEDORA = {coSeqPessoaDevedora}";

            using (IDbConnection connection = CodivaServiceConnection.GetConnection())
            {
                var result = connection.Execute(sql);

                return result > 0;
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
