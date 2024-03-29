﻿using Dapper;
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
                                                                              G.VL_TOTAL_GUIA          AS VALORTOTALGUIA,
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
                                                                       --AND    ROWNUM < 2
                                                                       --AND  to_date(G.DT_PAGAMENTO, 'dd/mm/yyyy') = to_date(sysdate - 1, 'dd/mm/yyyy')");

                return dadosGuiasPagas;
            }
        }

        public IEnumerable<DadosGuiaDto> ObterGuiasVencidas(string connectionString)
        {
            using (IDbConnection connection = PagamentoServiceConnection.GetConnection(connectionString))
            {
                var dadosGuiasVencidas = connection.Query<DadosGuiaDto>(@"SELECT DEB.NU_PROCESSO          AS NUMEROPROCESSO, 
                                                                                 G.CO_BOLETOBB_REGISTRADO AS NUMEROREFERENCIA,
                                                                                 PAG.DT_PAGAMENTO         AS DATAPAGAMENTO,
                                                                                 PAG.VL_PAGAMENTO         AS VALORPAGO,
                                                                                 G.VL_TOTAL_GUIA          AS VALORTOTALGUIA,
                                                                                 PARC.NU_NOSSO_NUMERO     AS NOSSONUMERO
                                                                          FROM   DBARRECAD.TB_GUIA G,
                                                                                 DBARRECAD.TB_BOLETOBB_REGISTRADO REG,
                                                                                 DBCODIVA.TB_PARCELA PARC,
                                                                                 DBCODIVA.TB_PAGAMENTO PAG,
                                                                                 DBCODIVA.TB_DEBITO DEB
                                                                          WHERE  G.CO_BOLETOBB_REGISTRADO = REG.CO_SEQ_BOLETOBB_REGISTRADO
                                                                          AND    REG.ID_SOLICITACAO = PARC.NU_NOSSO_NUMERO
                                                                          AND    PARC.CO_PAGAMENTO = PAG.NU_SEQ_PAGAMENTO (+)
                                                                          AND    PARC.CO_DEBITO = DEB.CO_SEQ_DEBITO
                                                                          AND    PARC.CO_PAGAMENTO IS NULL
                                                                          AND    ROWNUM < 5
                                                                          --AND    TO_DATE(G.DT_VENCIMENTO, 'dd/mm/yyyy') = TO_DATE(SYSDATE - 2, 'dd/mm/yyyy')
                                                                          --AND    DEB.ST_DEBITO_EXIGIVEL = 'N'");

                return dadosGuiasVencidas;
            }
        }
    }
}
