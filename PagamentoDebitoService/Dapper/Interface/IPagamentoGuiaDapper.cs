using PagamentoDebitoService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoDebitoService.Dapper.Interface
{
    public interface IPagamentoGuiaDapper
    {
        IEnumerable<DadosGuiaDto> ObterGuiasPagas(string connectionString);
        IEnumerable<DadosGuiaDto> ObterGuiasVencidas(string connectionString);
    }
}
