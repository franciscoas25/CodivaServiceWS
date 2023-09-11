using Dapper;
using PagamentoDebitoService.Connection;
using PagamentoDebitoService.Dapper.Interface;
using PagamentoDebitoService.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoDebitoService.Dapper.Implementation
{
    public class PagamentoGuiaDapper : IPagamentoGuiaDapper
    {
        public IEnumerable<DadosGuiaDto> ObterGuiasPagas(string connectionString)
        {
            using (IDbConnection connection = PagamentoServiceConnection.GetConnection(connectionString))
            {
                var dadosGuiasPagas = connection.Query<DadosGuiaDto>(@"SELECT NU_PROCESSO            AS NUMEROPROCESSO,
                                                                        CO_BOLETOBB_REGISTRADO AS NUMEROREFERENCIA,
                                                                        DT_PAGAMENTO           AS DATAPAGAMENTO,
                                                                        VL_TOTAL_GUIA          AS VALORPAGO
                                                                 FROM   DBARRECAD.TB_GUIA
                                                                 WHERE  ST_GUIA IN ('P', 'O', 'A', 'S', 'N', 'I')
                                                                 AND    to_date(DT_PAGAMENTO, 'dd/mm/yyyy') = to_date(sysdate - 4, 'dd/mm/yyyy')");

                return dadosGuiasPagas;
            }
        }

        public IEnumerable<DadosGuiaDto> ObterGuiasVencidas(string connectionString)
        {
            using (IDbConnection connection = PagamentoServiceConnection.GetConnection(connectionString))
            {
                var dadosGuiasVencidas = connection.Query<DadosGuiaDto>(@"SELECT NU_PROCESSO             AS NUMEROPROCESSO,
                                                                                 CO_BOLETOBB_REGISTRADO  AS NUMEROREFERENCIA,
                                                                                 DT_VENCIMENTO           AS DATAVENCIMENTO
                                                                          FROM   DBARRECAD.TB_GUIA
                                                                          WHERE  ST_GUIA NOT IN ('P', 'O', 'A', 'S', 'N', 'I')
                                                                          AND    to_date(DT_VENCIMENTO, 'dd/mm/yyyy') = to_date(sysdate - 4, 'dd/mm/yyyy')");

                return dadosGuiasVencidas;
            }
        }
    }
}
