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
                var dadosGuiasPagas = connection.Query<DadosGuiaDto>(@"SELECT DEB.NU_PROCESSO          AS NUMEROPROCESSO, 
                                                                              G.CO_BOLETOBB_REGISTRADO AS NUMEROREFERENCIA,
                                                                              PAG.DT_PAGAMENTO         AS DATAPAGAMENTO,
                                                                              PAG.VL_PAGAMENTO         AS VALORPAGO,
                                                                              PARC.NU_NOSSO_NUMERO     AS NOSSONUMERO
                                                                       FROM   DBARRECAD.TB_GUIA G,
                                                                              DBARRECAD.TB_BOLETOBB_REGISTRADO REG,
                                                                              DBCODIVA.TB_PARCELA PARC,
                                                                              DBCODIVA.TB_PAGAMENTO PAG,
                                                                              DBCODIVA.TB_DEBITO DEB
                                                                       WHERE  G.CO_BOLETOBB_REGISTRADO = REG.CO_SEQ_BOLETOBB_REGISTRADO
                                                                       AND    REG.ID_SOLICITACAO = PARC.NU_NOSSO_NUMERO
                                                                       AND    PARC.CO_PAGAMENTO = PAG.NU_SEQ_PAGAMENTO
                                                                       AND    PARC.CO_DEBITO = DEB.CO_SEQ_DEBITO
                                                                       AND    PARC.CO_PAGAMENTO IS NOT NULL
                                                                       AND    DEB.ST_DEBITO_EXIGIVEL = 'N'
                                                                       --AND    ROWNUM < 10
                                                                       --AND  to_date(G.DT_PAGAMENTO, 'dd/mm/yyyy') = to_date(sysdate - 1, 'dd/mm/yyyy')
                                                                       --AND    PARC.CO_DEBITO IN (33284, 32780)");

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
                                                                          AND    to_date(DT_VENCIMENTO, 'dd/mm/yyyy') = to_date(sysdate - 1, 'dd/mm/yyyy')");

                return dadosGuiasVencidas;
            }
        }
    }
}
